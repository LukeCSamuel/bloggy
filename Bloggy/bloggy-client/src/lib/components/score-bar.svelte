<script lang="ts">
    import { formatRelative } from 'date-fns';
  import type { SuccessfulCompletion } from "../entities/completion";
  import type { User } from "../entities/user";
  import ChevronLeftCircle from "../icons/chevron-left-circle.svelte";
  import Author from "./author.svelte";

  interface Props {
    user: User;
    completions: SuccessfulCompletion[];
    max: number;
  }

  let { user, completions, max }: Props = $props();

  let total = $derived(
    completions.reduce((i, completion) => i + completion.pointsAwarded, 0),
  );
  let isOpen = $state(false);

  function toggleOpen() {
    isOpen = !isOpen;
  }
</script>

<div class="col-start-1 col-span-1 font-slab text-3xl">
  {total}
</div>
<div
  class="col-start-2 col-span-1 pb-4 border-b-[1px] border-solid border-gray-300 last:border-b-0"
>
  <!-- svelte-ignore a11y_click_events_have_key_events -->
  <!-- svelte-ignore a11y_no_static_element_interactions -->
  <div class="flex" onclick={toggleOpen}>
    <Author author={user} format="compact" />
    <div class="grow"></div>
    <button
      class="c-square-icon h-full aspect-square flex justify-center items-center"
      class:rotate-90={isOpen}
      class:-rotate-90={!isOpen}
      onclick={toggleOpen}
    >
      <ChevronLeftCircle />
    </button>
  </div>
  <div class="my-2 flex h-2 bg-gray-300 rounded-full overflow-clip">
    <div
      class="h-full bg-purple-700 rounded-full"
      style:width={`${(total / max) * 100}%`}
    ></div>
  </div>
  <!-- Details about score -->
  {#if isOpen}
  {#each completions as completion}
    <div class="px-4 my-2 flex gap-2 text-xs border-l-[1px] border-solid border-gray-300 rounded-bl-sm">
      <strong class="font-slab">+{completion.pointsAwarded}</strong>
      <div>{completion.name}</div>
      <div class="grow"></div>
      <div class="text-gray-600">{completion.time && formatRelative(new Date(completion.time), new Date())}</div>
    </div>
  {/each}
  {/if}
</div>

<style lang="css">
  @reference "../../app.css";

  .c-square-icon :global(svg) {
    width: --spacing(6);
    height: --spacing(6);
  }
</style>
