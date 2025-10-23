<script lang="ts">
  import RouterOutlet from "../router/router-outlet.svelte";
  import Logo from "../icons/logo.svelte";
  import Link from "../router/link.svelte";
  import Footer from "../components/footer.svelte";
</script>

<header
  class="sticky top-0 flex items-center justify-center md:justify-between p-4 backdrop-blur-md"
>
  <Link route={{ name: "bloggy/home" }} class="h-8">
    <Logo class="w-auto h-full fill-gray-900" />
  </Link>
</header>

<RouterOutlet />

<Footer />
