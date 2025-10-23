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
    {/each}
  </div>
{:catch}
  <div>Error while loading posts.</div>
{/await}
