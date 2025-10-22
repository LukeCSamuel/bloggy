<script lang="ts">
  import Logo from "./icons/logo.svelte";
  import Link from "./router/link.svelte";
  import RouterOutlet from "./router/router-outlet.svelte";

  // TODO: when hitting the bloggy layout, check that the user is logged in
  //       if not, redirect them to the account creation page
</script>

<header
  class="sticky top-0 flex items-center justify-between px-4 py-2 backdrop-blur-md"
>
  <Link route={{ name: "bloggy/home" }} class="h-8">
    <Logo format="short" class="w-auto h-full fill-gray-900" />
  </Link>
  <!-- TODO: search component -->
  <!-- TODO: user icon component -->
</header>

<RouterOutlet />
