<script lang="ts">
  import type { Post, Comment, PostDto } from "../entities/post";
  import { getUserAsync, type User } from "../entities/user";
  import SvelteMarkdown from "@humanspeak/svelte-markdown";
  import Link from "../router/link.svelte";

  interface Props {
    post: PostDto;
  }

  let { post: dto }: Props = $props();

  let post = $derived(dto.post);
  let comments = $derived(dto.comments);
  let authors = $derived(dto.authors);
  let postAuthor = $derived(getAuthor(post));

  function getAuthor({ ownerId }: Post | Comment): User {
    return authors.find((author) => author.id === ownerId)!;
  }
</script>

<div>
  <h3>{post.title}</h3>
  <img src={postAuthor.pfpUrl} alt="{postAuthor.name}, author" />
  <div>
    by <Link
      route={{ name: "bloggy/profile", params: { userId: postAuthor.ownerId } }}
    >
      {postAuthor.name}
    </Link> at {new Date(post.created)}
  </div>
  {#each post.images as image}
    <img alt="unknown" src={image} />
  {/each}
  <SvelteMarkdown source={post.text} />

  <div>
    {#each comments as comment}
      {@const commentAuthor = getAuthor(comment)}
      <img alt={commentAuthor.name} src={commentAuthor.pfpUrl} />
      <div>{commentAuthor.name} at {new Date(comment.created)}</div>
      <div>{comment.text}</div>
    {/each}
  </div>
</div>
