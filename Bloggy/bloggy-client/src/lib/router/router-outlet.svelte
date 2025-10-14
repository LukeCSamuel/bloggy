<script lang="ts">
  import { getContext, setContext } from 'svelte';
  import { getRouter } from './configure-routes.svelte';

  const router = getRouter();
  const ROUTER_NESTING_INDEX = 'router-nesting-index';
  const index = getContext<number>(ROUTER_NESTING_INDEX) ?? 0;

  const View = $derived(router.route.views.at(index) ?? router.route.views.at(-1));

  setContext(ROUTER_NESTING_INDEX, index + 1);
</script>

<View />
