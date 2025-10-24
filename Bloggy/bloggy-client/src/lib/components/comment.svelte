<script lang="ts">
  import SvelteMarkdown from "@humanspeak/svelte-markdown";
  import { addCommentAsync, type Comment } from "../entities/post";
  import type { User } from "../entities/user";
  import Author from "./author.svelte";
  import { auth } from "../utils/auth.svelte";

  type Props =
    | {
        comment: Comment;
        author: User;
        postId?: undefined;
        refresh?: undefined;
      }
    | {
        comment?: undefined;
        author?: undefined;
        postId: string;
        refresh: () => void;
      };

  let props: Props = $props();

  let comment = $derived(props.comment);
  let author = $derived(props.author ?? auth.user!);

  let newText = $state("");
  let posting = $state(false);
  let canPostComment = $derived(!!newText?.trim() && !posting);

  async function addComment() {
    if (!canPostComment) {
      return;
    }

    if (props.postId) {
      posting = true;
      comment = await addCommentAsync(props.postId, { text: newText });
      props.refresh();
    }
  }
</script>

<div
  class="my-2 px-2 border-l-[1px] border-solid border-gray-300 rounded-bl-lg"
>
  <Author {author} date={comment?.created} format="compact" />
  <div class="pl-10 -mt-1 text-sm text-gray-900">
    {#if comment}
      <!-- Display comment that already exists -->
      <SvelteMarkdown source={comment.text} />
    {:else if props.postId}
      <!-- Allow comment creation -->
      <div class="relative">
        {#if !canPostComment}
          <div class="absolute top-0 left-0 text-gray-500 pointer-events-none">
            Add a comment...
          </div>
        {/if}
        <div
          class="c-edit w-full mb-2"
          contenteditable="true"
          bind:textContent={newText}
        ></div>
      </div>
      {#if canPostComment}
        <div class="flex justify-end">
          <button
            class="c-btn px-3 py-0.5 bg-yellow-300 z-10"
            onclick={addComment}
          >
            Post
          </button>
        </div>
      {/if}
    {/if}
  </div>
</div>
