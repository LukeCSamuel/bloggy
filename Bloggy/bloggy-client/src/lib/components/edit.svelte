<script lang="ts">
  import type { Snippet } from "svelte";

  interface Props {
    placeholder: string | Snippet;
    value: string;
    class?: string;
  }

  let {
    placeholder,
    value = $bindable(""),
    class: classList = "",
  }: Props = $props();

  let dirty = $derived(!!value.trim());
</script>

<div class="relative {classList}">
  {#if !dirty}
    <div class="absolute top-0 left-0 text-gray-500 pointer-events-none">
      {#if typeof placeholder === "string"}
        {placeholder}
      {:else}
        {@render placeholder()}
      {/if}
    </div>
  {/if}
  <div contenteditable="true" bind:textContent={value}></div>
</div>
