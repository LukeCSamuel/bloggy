<script lang="ts">
  import SvelteMarkdown from "@humanspeak/svelte-markdown";
  import { updateUserAsync, type User } from "../entities/user";
  import { auth } from "../utils/auth.svelte";

  interface Props {
    user: User;
  }

  let { user }: Props = $props();

  let about = $state(user.about ?? "");
  let canEdit = $derived(user.id === auth.user?.id);
  let dirty = $derived(!!about.trim() && about !== user.about);
  let editBox = $state(undefined) as HTMLElement | undefined;

  function edit() {
    if (editBox && !editBox.matches(':focus')) {
      editBox.focus();
    }
  }

  async function update() {
    if (!dirty) {
      return;
    }

    const result = await updateUserAsync(user.id, {
      ...user,
      about,
    });
    if (result) {
      user = result;
    }
  }
</script>

<!-- svelte-ignore a11y_click_events_have_key_events -->
<!-- svelte-ignore a11y_no_static_element_interactions -->
<div class="group relative">
  <div
    class="absolute top-0 left-0 group-focus-within:opacity-0 group-focus-within:pointer-events-none"
    class:cursor-pointer={canEdit}
    onclick={edit}
  >
    {#if about}
      <SvelteMarkdown source={about} />
    {:else if canEdit}
      <div class="text-gray-500">Add a profile description...</div>
    {/if}
  </div>
  {#if canEdit}
    <div
      contenteditable="true"
      bind:textContent={about}
      bind:this={editBox}
    ></div>
  {/if}
  {#if dirty}
    <div class="mt-2 flex justify-end">
      <button class="c-btn px-3 py-0.5 bg-yellow-300 z-10" onclick={update}>
        Update
      </button>
    </div>
  {/if}
</div>
