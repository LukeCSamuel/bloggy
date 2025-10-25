<script lang="ts">
  import SvelteMarkdown from "@humanspeak/svelte-markdown";
  import Loader from "../components/loader.svelte";
  import Post from "../components/post.svelte";
  import { getPostsForUserAsync } from "../entities/post";
  import type { User } from "../entities/user";
  import UserIcon from "../icons/user.svelte";
  import { getRouter } from "../router/configure-routes.svelte";
  import { api } from "../utils/api";
  import { auth } from "../utils/auth.svelte";
  import About from "../components/about.svelte";

  interface RouteParams {
    userId: string;
  }

  const router = getRouter<RouteParams>();

  let userPromise = $derived(
    api.getJsonAsync<User>(`api/user/${router.route.params.userId}`),
  );

  let postsPromise = $derived(getPostsForUserAsync(router.route.params.userId));

  router.setMeta({
    tabName,
  });
</script>

{#snippet tabName()}
  <Loader promise={userPromise}>
    {#snippet success(user)}
      <UserIcon /> {user?.name ?? "Unknown"}
    {/snippet}
  </Loader>
{/snippet}

<Loader promise={userPromise}>
  {#snippet success(user)}
    <div class="m-2 p-2 border-[1px] border-solid border-slate-800 rounded-md">
      <div class="flex gap-4">
        <div
          class="w-24 h-24 shrink-0 rounded-full overflow-clip border-2 border-solid border-gray-100 drop-shadow-sm"
        >
          <img alt={user.name} src={user.pfpUrl} />
        </div>
        <div class="grow">
          <h1 class="font-slab text-3xl">{user.name}</h1>
          <About {user} />
        </div>
      </div>
    </div>

    <Loader promise={postsPromise}>
      {#snippet success(posts)}
        {#each posts as post}
          <Post {post} />
        {/each}
      {/snippet}
    </Loader>
  {/snippet}
</Loader>
