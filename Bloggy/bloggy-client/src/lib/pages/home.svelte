<script lang="ts">
  import Post from "../components/post.svelte";
  import { getHomePostsAsync } from "../entities/post";

  let postsPromise = $derived(getHomePostsAsync());
</script>

{#await postsPromise}
  <div>Loading...</div>
{:then posts}
  <div>
    {#each posts as post}
      <Post {post} />
      <div
        class="last:hidden mx-16 pb-6 mb-6 border-b-[1px] border-solid border-slate-900"
      ></div>
    {/each}
  </div>
{:catch}
  <div>Error while loading posts.</div>
{/await}
