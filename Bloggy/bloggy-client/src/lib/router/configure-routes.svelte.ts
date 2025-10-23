/* eslint-disable svelte/prefer-svelte-reactivity */
import { getContext, setContext, type Component } from 'svelte';

export type ParamMatch<TValue> =
  | { isMatch: true; raw: string; value: TValue }
  | { isMatch: false };

export type RouteSubMatch<TParameters extends object> =
  | { isMatch: true; raw: string; params: TParameters }
  | { isMatch: false };

export type ParamMatcher<TValue> = (remaining: string) => ParamMatch<TValue>;

export type RouteMatcher<TParameters extends object> = {
  match: (remaining: string) => RouteSubMatch<TParameters>;
  build: (parameters: object) => string;
};

export class Param<TName extends string | symbol, TValue> {
  constructor (private _name: TName, private _matcher: ParamMatcher<TValue>) {}

  get name () {
    return this._name;
  }

  tryMatch (remaining: string) {
    return this._matcher(remaining);
  }
}

const segmentRegex = /^[^/?]+/;
const numberRegex = /^\d+/;

/**
 * Creates a route parameter definition
 * @param name The name of the parameter to create
 */
export function param<TName extends string | symbol> (name: TName) {
  return {
    match (regex: RegExp) {
      return new Param(name, (remaining): ParamMatch<string> => {
        const [raw] = regex.exec(remaining) ?? [];
        return raw === undefined
          ? {
              isMatch: false,
            }
          : {
              isMatch: true,
              raw,
              value: raw,
            };
      });
    },
    matchCustom<TValue>(matcher: ParamMatcher<TValue>) {
      return new Param(name, matcher);
    },
    matchSegment () {
      return new Param(name, (remaining): ParamMatch<string> => {
        const [raw] = segmentRegex.exec(remaining) ?? [];
        return raw
          ? {
              isMatch: true,
              raw,
              value: raw,
            }
          : {
              isMatch: false,
            };
      });
    },
    matchNumber () {
      return new Param(name, (remaining): ParamMatch<number> => {
        const [raw] = numberRegex.exec(remaining) ?? [];
        return raw
          ? {
              isMatch: true,
              raw,
              value: Number(raw),
            }
          : {
              isMatch: false,
            };
      });
    },
  };
}

export interface IntermediateMatchedRoute<TParameters extends object> {
  name: string;
  params: TParameters;
  views: Component[];
  raw: string;
}

export interface MatchedRoute<TParameters extends object> extends IntermediateMatchedRoute<TParameters> {
  href: string;
  query: Record<string, string>;
}

/**
 * Represents a complete, matchable route
 */
export class Route<TParameters extends object> {
  constructor (
    private _name: string,
    private _subMatcher: RouteMatcher<TParameters>,
    private _view: Component,
    private _aliases?: RouteMatcher<TParameters>[],
    private _nestedRoutes?: Route<Partial<TParameters>>[],
  ) {
    if (_name.includes('/')) {
      throw new Error('Route name cannot contain slashes. Slashes are reserved for denoting nested routes.');
    }
  }

  get name () {
    return this._name;
  }

  tryMatch (href: string): MatchedRoute<TParameters> | false {
    const url = new URL(href);
    const remaining = url.pathname;

    console.log(`Matching route:"${this.name}" on path:"${remaining}"`);

    const matchResult = this.intermediateMatch(remaining);

    console.log(` > result:`, matchResult);

    if (matchResult) {
      const query = Object.fromEntries(url.searchParams.entries());
      // Rebuild the href using the canonical
      const canonicalPath = this.tryBuildHref(matchResult.name, matchResult.params, query, url.hash) || href;
      const canonicalHref = new URL(canonicalPath, document.baseURI).href;

      return {
        href: canonicalHref,
        query,
        ...matchResult,
      };
    } else {
      return false;
    }
  }

  tryBuildHref (name: string, parameters: TParameters, query?: Record<string, string>, fragment: string = ''): string | false {
    const [routeName, ...subRouteNames] = name.split('/');
    if (routeName !== this.name) {
      return false;
    }

    try {
      const path = this.intermediateBuildHref(subRouteNames, parameters);
      const searchParams = new URLSearchParams(query);
      const href = `${path}${searchParams.size > 0 ? '?' + searchParams.toString() : ''}${fragment && '#' + fragment}`;
      return href;
    } catch (error_) {
      const error = error_ instanceof Error ? new Error(`Failed to build href for route "${name}": ${error_.message}`) : error_;
      throw error;
    }
  }

  private intermediateBuildHref ([child, ...remaining]: string[], parameters: TParameters): string {
    const partialPath = this._subMatcher.build(parameters);
    if (child) {
      const route = this._nestedRoutes?.find(r => r.name === child);
      if (!route) {
        throw new Error(`Missing nested route "${child}"`);
      }

      const nestedPath = route.intermediateBuildHref(remaining, parameters);
      return `${partialPath}${nestedPath}`;
    } else {
      return partialPath;
    }
  }

  private getMatchWithAliases (remaining: string): RouteSubMatch<TParameters> {
    let matchResult = this._subMatcher.match(remaining);
    // Try matching aliases if the canonical route failed
    if (!matchResult.isMatch) {
      for (const alias of this._aliases ?? []) {
        matchResult = alias.match(remaining);
        if (matchResult.isMatch) {
          return matchResult;
        }
      }
    }
    return matchResult;
  }

  private intermediateMatch (remaining: string): IntermediateMatchedRoute<TParameters> | false {
    const matchResult = this.getMatchWithAliases(remaining);

    console.log(` > matching sub-route:"${this.name}" on path:"${remaining}"`, matchResult);

    if (!matchResult.isMatch) {
      return false;
    }

    const newRemaining = remaining.replace(matchResult.raw, '');

    if (this._nestedRoutes?.length) {
      for (const route of this._nestedRoutes ?? []) {
        const nestedMatch = route.intermediateMatch(newRemaining);
        // Require that all of the path has been matched
        if (nestedMatch) {
          return {
            name: `${this.name}/${nestedMatch.name}`,
            params: {
              ...matchResult.params,
              ...nestedMatch.params,
            },
            views: [this._view, ...nestedMatch.views],
            raw: remaining,
          };
        }
      }
    }

    // Nothing left to match, except possibly a trailing '/'
    if (/^\/?$/.test(newRemaining) && !this._nestedRoutes?.length) {
      return {
        name: this.name,
        params: matchResult.params,
        views: [this._view],
        raw: remaining + newRemaining,
      };
    }

    // Could not match any nested routes either
    return false;
  }
}

export type ParamToType<T> = T extends Param<infer TKey, infer TValue> ? { [K in TKey]: TValue } : object;

export function match (strings: TemplateStringsArray): RouteMatcher<object>;
export function match<TParameter0 extends Param<string | symbol, unknown>> (strings: TemplateStringsArray, parameter0: TParameter0): RouteMatcher<ParamToType<TParameter0>>;
export function match (strings: TemplateStringsArray, ...parameters: Param<string | symbol, unknown>[]): RouteMatcher<object> {
  return {
    match: (remaining: string): RouteSubMatch<object> => {
      const matchedParameters = {} as Record<string | symbol, unknown>;
      let raw = '';

      for (const [index, string_] of strings.entries()) {
        if (!remaining.startsWith(string_)) {
          return { isMatch: false };
        }
        raw += string_;
        remaining = remaining.replace(string_, '');
        if (index < parameters.length) {
          // If there are still params left to match, try to match them
          const match = parameters[index].tryMatch(remaining);
          if (match.isMatch) {
            matchedParameters[parameters[index].name] = match.value;
            raw += match.raw;
            remaining = remaining.replace(match.raw, '');
          } else {
            return { isMatch: false };
          }
        } else {
          // There are no params left to match, so the match is successful
          return {
            isMatch: true,
            params: matchedParameters,
            raw,
          };
        }
      }

      // Function wasn't called correctly
      return { isMatch: false };
    },
    build: (params: object) => {
      const parts = [] as string[];
      for (const [index, string_] of strings.entries()) {
        parts.push(string_);
        if (index < parameters.length) {
          const param = parameters[index];
          const rawValue = String(params[param.name as keyof typeof params] ?? '');

          if (param.tryMatch(String(rawValue)).isMatch) {
            parts.push(rawValue);
          } else {
            throw new Error(`Parameter "${String(param.name)}" cannot accept the inferred value "${rawValue}"`);
          }
        }
      }
      return parts.join('');
    },
  };
}

export interface RouteDefinition {
  name: string;
  match: RouteMatcher<object> | RegExp | string;
  view: Component;
  /**
   * An alias is an alternative way to match a route that will be resolved to the canonical `match`
   */
  aliases?: (RouteMatcher<object> | RegExp | string)[];
  nested?: RouteDefinition[];
}

/**
 * Creates a route matcher from a regular expression
 * @param regexp A regular expression used the match the route
 */
function createMatcherFromRegExp (regexp: RegExp): RouteMatcher<object> {
  return {
    match: (remaining: string) => {
      const [raw] = regexp.exec(remaining) ?? [];
      // Check that the regex matches, and enforce that the match occurs at the beginning
      //   of the remaining string in case the regex doesn't
      return raw && remaining.startsWith(raw)
        ? {
            isMatch: true,
            raw,
            params: {},
          }
        : {
            isMatch: false,
          };
    },
    build: () => {
      throw new Error('Cannot build route from RegExp matcher');
    },
  };
}

/**
 * Creates a route matcher from a string
 * @param raw A plain string used to match the route
 */
function createMatcherFromString (raw: string): RouteMatcher<object> {
  return {
    match: (remaining: string) => {
      return remaining.startsWith(raw)
        ? {
            isMatch: true,
            raw,
            params: {},
          }
        : {
            isMatch: false,
          };
    },
    build: () => raw,
  };
}

function getMatcher (match: RouteDefinition['match']): RouteMatcher<object> {
  return match instanceof RegExp
    ? createMatcherFromRegExp(match)
    : (typeof match === 'string'
        ? createMatcherFromString(match)
        : match);
}

/**
 * Defines routes to be used by the app
 * @param routeDefinitions Route configuration
 */
export function defineRoutes<T extends RouteDefinition[]> (routeDefinitions: T): Route<object>[] {
  const routes = [];
  for (const route of routeDefinitions) {
    const nested = route.nested ? defineRoutes(route.nested) : undefined;
    const match = getMatcher(route.match);
    const aliases = route.aliases?.map(alias => getMatcher(alias)) ?? [];
    routes.push(new Route(
      route.name,
      match,
      route.view,
      aliases,
      nested,
    ));
  }
  return routes;
}

const ROUTE_CONTEXT = 'router-context';

export type Navigable =
  | string
  | { href: string }
  | {
    name: string;
    params?: object;
    query?: Record<string, string>;
    fragment?: string;
  };

export type ResolvedRoute = {
  href: string;
  isFragment: boolean;
  isExternal: boolean;
  isActive: boolean;
};

export interface RouterContext<TParameters extends object> {
  /**
   * Information about the current route
   */
  route: MatchedRoute<TParameters>;
  /**
   * Navigates to a specified href or route
   * @param target Destination to which the app should navigate
   */
  navigateTo: (target: Navigable) => void;
  /**
   * Resolves the given target into a base-relative href
   * @param target An href, or an object used to construct a route
   */
  resolveRoute: (target: Navigable) => ResolvedRoute;
}

/**
 * Gets a reference to the router from the Svelte context
 */
export function getRouter<TParameters extends object = object> () {
  return getContext<RouterContext<TParameters>>(ROUTE_CONTEXT);
}

/**
 * Manages state surrounding App Routing
 */
export class AppRouter implements RouterContext<object> {
  private _routes: Route<object>[];
  private _state: {
    route: MatchedRoute<object>;
  };

  route: MatchedRoute<object>;

  constructor (routeDefinitions: RouteDefinition[]) {
    this._routes = defineRoutes(routeDefinitions);
    this._state = $state({
      route: AppRouter.defaultRoute(),
    });
    this.route = $derived(this._state.route);

    if (AppRouter.appRouter) {
      AppRouter.appRouter.stop();
    }
    AppRouter.appRouter = this;
  }

  /**
   * Resolves the given target into a base-relative href
   * @param target An href, or an object used to construct a route
   */
  resolveRoute (target: Navigable): ResolvedRoute {
    if (typeof target === 'string' || 'href' in target) {
      const href = typeof target === 'string' ? target : target.href;
      if (href.startsWith('#')) {
        // Construct a full URL for fragment-only hrefs relative to the current route
        // Remove existing fragment, if there is one
        const [baseHref] = this._state.route.href.split('#');
        return this.createResolvedRoute(baseHref + href);
      } else {
        return this.createResolvedRoute(href);
      }
    } else {
      // Construct href from route name and parameters
      const href = this._routes.map(route => route.tryBuildHref(target.name, target.params || {}, target.query, target.fragment)).find(Boolean);
      if (href) {
        return this.createResolvedRoute(href);
      } else {
        throw new Error(`No route configured for "${target.name}"`);
      }
    }
  }

  /**
   * Navigates to a specified href or route
   * @param target Destination to which the app should navigate
   */
  navigateTo (target: Navigable) {
    const { href } = this.resolveRoute(target);
    this.matchRoute(href);
  }

  onPopState = (event: PopStateEvent) => {
    const { url } = event.state ?? {};
    if (url instanceof URL) {
      this.matchRoute(url.href);
    } else if (typeof url === 'string') {
      this.matchRoute(url);
    }
  };

  start () {
    setContext(ROUTE_CONTEXT, this);
    this.matchRoute();
    globalThis.window.addEventListener('popstate', this.onPopState);

    // Update browser history when route href changes
    let oldUrl = new URL(this._state.route.href, globalThis.location.href);
    $effect(() => {
      const newUrl = new URL(this._state.route.href, globalThis.location.href);

      if (newUrl.href === globalThis.history.state?.url) {
        // Don't modify the state, it's up to date
        return;
      } else if (newUrl.pathname === oldUrl.pathname) {
        globalThis.history.replaceState({ url: newUrl.href }, '', newUrl.href);
      } else {
        globalThis.history.pushState({ url: newUrl.href }, '', newUrl.href);
      }

      oldUrl = newUrl;
    });
  }

  matchRoute (href: string = globalThis.location.href): { routeChanged: boolean } {
    const url = new URL(href, globalThis.location.href);
    const route = this._routes.map(route => route.tryMatch(url.href)).find(Boolean) || AppRouter.defaultRoute(href);
    if (route.name === '*') {
      globalThis.console.warn('Failed to match route for', url.href);
    }

    const routeChanged = route.name !== this._state.route.name;

    this._state.route = route;

    // If navigating to a different route, scroll to top OR to specified fragment
    if (routeChanged) {
      if (url.hash) {
        // Set a timeout to resolve the element later, after the DOM has updated
        setTimeout(() => {
          const element = document.querySelector(url.hash);
          if (element) {
            element.scrollIntoView({ behavior: 'smooth' });
          } else {
            globalThis.window.scrollTo(0, 0);
          }
        }, 0);
      } else {
        globalThis.window.scrollTo(0, 0);
      }
    }

    return {
      routeChanged,
    };
  }

  stop () {
    globalThis.window.removeEventListener('popstate', this.onPopState);
  }

  /**
   * Creates a ResolvedRoute for the given href, providing additional metadata
   * @param href The href to create a ResolvedRoute for
   */
  private createResolvedRoute (href: string): ResolvedRoute {
    const url = new URL(href, document.baseURI);
    const [prefragment, fragment] = url.href.split('#');

    const isActive = this._state.route.href.startsWith(prefragment);
    const isFragment = isActive && fragment !== undefined;
    const isExternal = !url.href.startsWith(document.baseURI);

    return {
      href,
      isActive,
      isFragment,
      isExternal,
    };
  }

  private static defaultRoute (href: string = globalThis.location.href): MatchedRoute<object> {
    const url = new URL(href, globalThis.location.href);
    return {
      name: '*',
      views: [],
      params: {},
      raw: '',
      href: url.href,
      query: Object.fromEntries(url.searchParams.entries()),
    };
  }

  private static appRouter: AppRouter | undefined;
}
