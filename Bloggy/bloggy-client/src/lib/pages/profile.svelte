<script lang="ts">
  import type { User } from "../entities/user";
  import UserIcon from "../icons/user.svelte";
  import { getRouter } from "../router/configure-routes.svelte";
  import { api } from "../utils/api";

  interface RouteParams {
    userId: string;
  }

  const router = getRouter<RouteParams>();

  let userPromise = $derived(
    api.getJsonAsync<User>(`api/user/${router.route.params.userId}`),
  );

  router.setMeta({
    tabName,
  });
</script>

{#snippet tabName()}
  {#await userPromise}
    Loading...
  {:then user}
    <UserIcon /> {user?.name ?? 'Unknown'}
  {/await}
{/snippet}

{#await userPromise}
  Loading...
{:then user}
  {#if user}
    <img alt={user.name} src={user.pfpUrl} />
    <h1>{user.name}</h1>
  {:else}
    User does not exist.
  {/if}
{:catch}
  Error while loading user.
{/await}
