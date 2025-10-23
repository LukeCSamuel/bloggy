<script lang="ts">
  import Link from '../router/link.svelte';
</script>

<footer class="px-4 py-10 bg-gray-200">
  <!-- TODO: add github link with logo -->
  <div class="max-w-[640px] m-auto flex justify-between gap-8">
    <div class="flex flex-col">
      <Link route={{ name: "support/terms" }}>Terms of Service</Link>
      <Link route={{ name: "support/privacy" }}>Privacy Policy</Link>
      <Link route={{ name: "support/community-guidelines" }}>
        Community Guidelines
      </Link>
    </div>
    <div>© 2025 Bloggy</div>
  </div>
</footer>