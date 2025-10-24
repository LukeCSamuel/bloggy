<script lang="ts">
  import type { User } from "../entities/user";
  import Link from "../router/link.svelte";
  import { formatRelative } from 'date-fns';  

  interface Props {
    author: User;
    format: 'full' | 'compact'
    date?: string | Date;
  }

  let { author, date, format }: Props = $props();
  let formattedDate = $derived(date && formatRelative(new Date(date), new Date()));
</script>

<Link class="clear block group" route={{ name: "bloggy/profile", params: { userId: author.id } }}>
  <div class="grid {format} items-baseline gap-x-2 group-has-[.full]:text-right">
    <div class="area-pfp self-center rounded-full overflow-clip border-2 border-solid border-gray-100 drop-shadow-sm">
      <img src={author.pfpUrl} alt={author.name}>
    </div>
    <div class="area-name font-slab font-medium text-blue-800 hover:underline">
      {author.name}
    </div>
    {#if date}
      <div class="aria-date text-xs text-gray-600">
        {formattedDate}
      </div>
    {/if}
  </div>
</Link>

<style lang="css">
  @reference "../../app.css";

  .full {
    grid-template:
      "name pfp" auto
      "date pfp" auto
      / max-content --spacing(12);
  }

  .compact {
    grid-template:
      "pfp name date" auto
      / --spacing(8) max-content max-content;
  }

  .area-name {
    grid-area: name;
  }

  .area-pfp {
    grid-area: pfp;
  }

  .area-date {
    grid-area: date;
  }
</style>
