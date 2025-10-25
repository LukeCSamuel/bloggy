<script lang="ts">
  import SvelteMarkdown from "@humanspeak/svelte-markdown";
  import { isTextAlert, type Alert } from "../utils/alerts.svelte";
  import { isFailedCompletion } from "../entities/completion";
    import { fade, fly } from 'svelte/transition';
    import { cubicOut } from 'svelte/easing';

  interface Props {
    alert: Alert;
  }

  let { alert }: Props = $props();

  let isError = $derived(
    isTextAlert(alert)
      ? alert.kind === "error"
      : isFailedCompletion(alert.completion),
  );
</script>

<div
  class="fixed bottom-4 left-[50%] translate-x-[-50%] max-w-[90dvw] w-max px-4 py-1 text-center text-sm border-2 border-solid rounded-full shadow-lg z-50"
  class:bg-red-300={isError}
  class:text-red-950={isError}
  class:border-red-500={isError}
  class:bg-emerald-300={!isError}
  class:text-emerald-950={!isError}
  class:border-emerald-500={!isError}
  in:fly={{ duration: 300, easing: cubicOut, x: 0, y: 50 }}
  out:fade={{ duration: 150 }}
>
  {#if isTextAlert(alert)}
    {#if typeof alert.text === "string"}
      <SvelteMarkdown source={alert.text} />
    {:else}
      {@render alert.text()}
    {/if}
  {:else}
    {#if isFailedCompletion(alert.completion)}
      {#if alert.completion.isExpired}
        This event has expired.
      {:else}
        Try again!
      {/if}
    {:else}
      <div class="flex items-center gap-2">
        {#if alert.completion.pointsAwarded}
          <div class="shrink-0 font-slab font-semibold text-lg">+{alert.completion.pointsAwarded}</div>
        {/if}
        <div class="text-sm/tight text-left">
          {alert.completion.name}
        </div>
      </div>
    {/if}
  {/if}
</div>
