<script lang="ts">
  import RouterOutlet from "../router/router-outlet.svelte";
  import Logo from "../icons/logo.svelte";
  import Link from "../router/link.svelte";
</script>

<header
  class="sticky top-0 flex items-center p-4 backdrop-blur-md drop-shadow-md"
>
  <Link route={{ name: "bloggy/home" }} class="h-8">
    <Logo class="w-auto h-full fill-gray-900" />
  </Link>
</header>

<RouterOutlet />

<footer class="px-4 py-10 bg-gray-200">
  <!-- TODO: add github link with logo -->
  <div class="flex max-w-max mx-auto gap-8">
    <div class="flex flex-col">
      <Link route={{ name: "support/terms" }}>Terms of Service</Link>
      <Link route={{ name: "support/privacy" }}>Privacy Policy</Link>
      <Link route={{ name: "support/community-guidelines" }}>
        Community Guidelines
      </Link>
    </div>
    <div class="max-w-max mx-auto">© 2025 Bloggy</div>
  </div>
</footer>
