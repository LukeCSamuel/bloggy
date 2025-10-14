<script lang="ts">
  import { getRouter } from './router/configure-routes.svelte';

  const router = getRouter<{ path: string }>();
  const path = $derived(router.route.params.path)
</script>

<h1>Error</h1>
<p>
  The page you're trying to access, "{path}", doesn't exist.
</p>