<script lang="ts">
    import { tryCompletionAsync } from '../entities/completion';
    import { getRouter } from '../router/configure-routes.svelte';

  interface RouteParams {
    challengeId: string;
  }

  const router = getRouter<RouteParams>();

  let challengePromise = $derived(
    tryCompletionAsync({
      challengeId: router.route.params.challengeId,
      key: router.route.params.challengeId,
    })
  );
  let nextRoute = $derived(router.route.query.from || { name: 'bloggy/home' });

  $effect(() => {
    challengePromise.finally(() => {
      router.navigateTo(nextRoute);
    });
  })
</script>