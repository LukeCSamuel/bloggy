<script lang="ts">
  import Loader from "../components/loader.svelte";
  import Post from "../components/post.svelte";
  import { getTrendingPostsAsync } from "../entities/post";

  let postsPromise = $derived(getTrendingPostsAsync());
</script>

<Loader promise={postsPromise}>
  {#snippet success(posts)}
    <div>
      {#each posts as post}
        <Post {post} />
      {/each}
    </div>
  {/snippet}
</Loader>
