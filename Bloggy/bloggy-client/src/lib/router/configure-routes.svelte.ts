import { getContext, setContext, type Component } from 'svelte';

export type ParamMatch<TValue> =
  | { isMatch: true, raw: string, value: TValue }
  | { isMatch: false }

export type RouteSubMatch<TParams extends object> =
  | { isMatch: true, raw: string, params: TParams }
  | { isMatch: false }

export type ParamMatcher<TValue> = (remaining: string) => ParamMatch<TValue>

export type RouteMatcher<TParams extends object> = (remaining: string) => RouteSubMatch<TParams>

export class Param<TName extends string | symbol, TValue> {
  constructor (private _name: TName, private _matcher: ParamMatcher<TValue>) { }

  get name () {
    return this._name;
  }

  tryMatch (remaining: string) {
    return this._matcher(remaining);
  }
}

const segmentRegex = /^[^/\?]+/;
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
        if (raw !== undefined) {
          return {
            isMatch: true,
            raw,
            value: raw,
          };
        } else {
          return {
            isMatch: false,
          };
        }
      })
    },
    matchCustom<TValue> (matcher: ParamMatcher<TValue>) {
      return new Param(name, matcher);
    },
    matchSegment () {
      return new Param(name, (remaining): ParamMatch<string> => {
        const [raw] = segmentRegex.exec(remaining) ?? [];
        if (raw) {
          return {
            isMatch: true,
            raw,
            value: raw,
          };
        } else {
          return {
            isMatch: false,
          };
        }
      })
    },
    matchNumber () {
      return new Param(name, (remaining): ParamMatch<number> => {
        const [raw] = numberRegex.exec(remaining) ?? [];
        if (raw) {
          return {
            isMatch: true,
            raw,
            value: Number(raw),
          };
        } else {
          return {
            isMatch: false,
          };
        }
      });
    }
  };
}

export interface IntermediateMatchedRoute<TParams extends object> {
  name: string
  params: TParams
  views: Component[]
  raw: string
}

export interface MatchedRoute<TParams extends object> extends IntermediateMatchedRoute<TParams> {
  href: string
  query: Record<string, string>
}

/**
 * Represents a complete, matchable route
 */
export class Route<TParams extends object> {
  constructor (
    private _name: string,
    private _subMatcher: RouteMatcher<TParams>,
    private _view: Component,
    private _nestedRoutes?: Route<Partial<TParams>>[],
  ) {}

  get name () {
    return this._name;
  }

  tryMatch (href: string): MatchedRoute<TParams> | false {
    const url = new URL(href);
    const remaining = url.pathname;

    console.log(`Matching route:"${this.name}" on path:"${remaining}"`);

    const matchResult = this.intermediateMatch(remaining);

    console.log(` > result:`, matchResult);

    if (matchResult) {
      const query = Object.fromEntries(url.searchParams.entries());
      return {
        href,
        query,
        ...matchResult,
      };
    } else {
      return false;
    }
  }

  private intermediateMatch (remaining: string): IntermediateMatchedRoute<TParams> | false {
    const matchResult = this._subMatcher(remaining);

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

export type ParamToType<T> = T extends Param<infer TKey, infer TValue> ? { [K in TKey]: TValue } : {};


// TODO: add functionality to allow building a route URL when given params

export function match (strings: TemplateStringsArray): RouteMatcher<object>
export function match<TParam0 extends Param<string | symbol, unknown>> (strings: TemplateStringsArray, param0: TParam0): RouteMatcher<ParamToType<TParam0>>;
export function match (strings: TemplateStringsArray, ...params: Param<string | symbol, unknown>[]): RouteMatcher<object> {
  return (remaining: string): RouteSubMatch<object> => {
    const matchedParams = {} as Record<string | symbol, unknown>;
    let raw = '';

    for (let i = 0; i < strings.length; i++) {
      if (!remaining.startsWith(strings[i])) {
        return { isMatch: false };
      }
      raw += strings[i];
      remaining = remaining.replace(strings[i], '');
      if (i < params.length) {
        // If there are still params left to match, try to match them
        const match = params[i].tryMatch(remaining);
        if (match.isMatch) {
          matchedParams[params[i].name] = match.value;
          raw += match.raw;
          remaining = remaining.replace(match.raw, '');
        } else {
          return { isMatch: false };
        }
      } else {
        // There are no params left to match, so the match is successful
        return {
          isMatch: true,
          params: matchedParams,
          raw,
        };
      } 
    }

    // Function wasn't called correctly
    return { isMatch: false };
  }
}

export interface RouteDefinition {
  name: string
  match: RouteMatcher<object> | RegExp | string
  view: Component
  nested?: RouteDefinition[]
}

/**
 * Creates a route matcher from a regular expression
 * @param regexp A regular expression used the match the route
 */
function createMatcherFromRegExp (regexp: RegExp): RouteMatcher<object> {
  return (remaining: string) => {
    const [raw] = regexp.exec(remaining) ?? [];
    // Check that the regex matches, and enforce that the match occurs at the beginning
    //   of the remaining string in case the regex doesn't
    if (raw && remaining.startsWith(raw)) {
      return {
        isMatch: true,
        raw,
        params: {},
      };
    } else {
      return {
        isMatch: false,
      };
    }
  };
}

/**
 * Creates a route matcher from a string
 * @param raw A plain string used to match the route
 */
function createMatcherFromString (raw: string): RouteMatcher<object> {
  return (remaining: string) => {
    if (remaining.startsWith(raw)) {
      return {
        isMatch: true,
        raw,
        params: {},
      };
    } else {
      return {
        isMatch: false,
      };
    }
  };
}

/**
 * Defines routes to be used by the app
 * @param routeDefinitions Route configuration
 */
export function defineRoutes<T extends RouteDefinition[]> (routeDefinitions: T): Route<object>[] {
  const routes = [];
  for (const route of routeDefinitions) {
    const nested = route.nested ? defineRoutes(route.nested) : undefined;
    const match = route.match instanceof RegExp 
      ? createMatcherFromRegExp(route.match)
      : typeof route.match === 'string'
      ? createMatcherFromString(route.match)
      : route.match;
    routes.push(new Route(
      route.name,
      match,
      route.view,
      nested,
    ));
  }
  return routes;
}

const ROUTE_CONTEXT = 'router-context';

export interface RouterContext<TParams extends object> {
  route: MatchedRoute<TParams>
  navigateTo: (href: string) => void
}

/**
 * Gets a reference to the router from the Svelte context
 */
export function getRouter<TParams extends object = object> () {
  return getContext<RouterContext<TParams>>(ROUTE_CONTEXT);
}

/**
 * Manages state surrounding App Routing
 */
export class AppRouter implements RouterContext<object> {
  private _routes: Route<object>[];
  private _state: {
    route: MatchedRoute<object>
  }

  route: MatchedRoute<object>

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

  navigateTo (href: string) {
    this.matchRoute(href);
  }

  onPopState = (event: PopStateEvent) => {
    const { url } = event.state;
    if (url instanceof URL) {
      this.matchRoute(url.href);
    } else if (typeof url === 'string') {
      this.matchRoute(url);
    }
  }

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

  matchRoute (href: string = globalThis.location.href) {
    const url = new URL(href, globalThis.location.href);
    const route = this._routes.map(route => route.tryMatch(url.href)).find(Boolean) || AppRouter.defaultRoute(href);
    if (route.name === '*') {
      globalThis.console.warn('Failed to match route for', url.href);
    }

    this._state.route = route;
  }

  stop () {
    globalThis.window.removeEventListener('popstate', this.onPopState);
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
