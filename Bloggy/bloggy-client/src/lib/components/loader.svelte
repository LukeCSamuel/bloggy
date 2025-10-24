<script lang="ts" generics="T">
  import type { Snippet } from "svelte";

  interface Props {
    promise: Promise<T>;
    success: Snippet<[NonNullable<T>]>;
    missing?: Snippet;
    error?: Snippet;
  }

  let { promise, success, missing, error }: Props = $props();
</script>

{#await promise}
  Loading...
{:then result}
  {#if result}
    {@render success(result)}
  {:else if missing}
    {@render missing()}
  {:else}
    An error occurred.
  {/if}
{:catch}
  {#if error}
    {@render error()}
  {:else}
    An error occurred.
  {/if}
{/await}
