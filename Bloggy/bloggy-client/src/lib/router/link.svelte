<script lang="ts">
  import type { Snippet } from "svelte";
  import {
    getRouter,
    type Navigable,
    type ResolvedRoute,
  } from "./configure-routes.svelte";

  const router = getRouter();

  type Props = {
    href?: string;
    route?: Navigable;
    children: Snippet<[ResolvedRoute]>;
    class?: string;
    target?: "_blank" | "_self";
  };

  let {
    href: _passedHref = "",
    route = _passedHref,
    children,
    class: classList,
    target,
  }: Props = $props();

  let resolvedRoute = $derived(router.resolveRoute(route));

  function onclick(event: MouseEvent) {
    if (
      resolvedRoute.isFragment ||
      resolvedRoute.isExternal ||
      target === "_blank"
    ) {
      // Allow the default behavior for fragment links
      return;
    } else {
      event.preventDefault();
      router.navigateTo(resolvedRoute.href);
    }
  }
</script>

<a
  href={resolvedRoute.href}
  {onclick}
  class={classList}
  class:active={resolvedRoute.isActive}
  {target}
>
  {#if children}
    {@render children(resolvedRoute)}
  {:else}
    {new URL(resolvedRoute.href, document.baseURI).toString()}
  {/if}
</a>
