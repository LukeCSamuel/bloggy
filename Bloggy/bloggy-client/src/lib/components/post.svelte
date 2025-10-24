<script lang="ts">
  import { getPostAsync, type PostDto } from "../entities/post";
  import { type User } from "../entities/user";
  import SvelteMarkdown from "@humanspeak/svelte-markdown";
  import Author from "./author.svelte";
  import Comment from "./comment.svelte";
  import type { EntityBase } from "../entities/entity-base";
  import { auth } from "../utils/auth.svelte";

  interface Props {
    post: PostDto;
  }

  let { post: dto }: Props = $props();

  let post = $derived(dto.post);
  let comments = $derived(dto.comments);
  let authors = $derived(dto.authors);
  let postAuthor = $derived(getAuthor(post));
  let refreshes = $state(0);

  function getAuthor({ ownerId }: EntityBase): User {
    return authors.find((author) => author.id === ownerId)!;
  }
  
  async function refresh() {
    const result = await getPostAsync(post.id);
    if (result) {
      dto = result;
      refreshes++;
    }
  }
</script>

<div class="p-4">
  <div class="flex justify-between">
    <h3 class="font-slab text-xl">{post.title}</h3>
    <Author author={postAuthor} date={post.created} format="full" />
  </div>
  <!-- TODO: carousel -->
  <div class="-mx-4">
    {#each post.images as image}
      <img alt="unknown" src={image} />
    {/each}
  </div>
  <div class="py-4 border-b-[1px] border-solid border-gray-300">
    <SvelteMarkdown source={post.text} />
  </div>

  {#each comments as comment}
    <Comment {comment} author={getAuthor(comment)} />
  {/each}
  {#key refreshes}
    <Comment postId={post.id} {refresh} />
  {/key}
</div>
