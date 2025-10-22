<script lang="ts">
  import ProfilePicture from "../components/profile-picture.svelte";
  import { uploadPfpAsync } from "../entities/user";
  import { getRouter } from "../router/configure-routes.svelte";
  import Link from "../router/link.svelte";
  import { auth, registerAsync } from "../utils/auth.svelte";

  const router = getRouter();
  const nextRoute = router.route.query.from || { name: 'bloggy/home' };
  
  // if user is already registered, go back to "from" URL
  if (auth.isAuthenticated) {
    setTimeout(() => router.navigateTo(nextRoute), 0);
  }

  let blob: Blob | undefined = $state(undefined);
  let name: string = $state("");
  let agreed: boolean = $state(false);

  let isValid = $derived(!!blob && !!name && !!agreed);

  async function signUp() {
    if (isValid) {
      await registerAsync({ name });
      await uploadPfpAsync(blob!);

      if (auth.isAuthenticated) {
        router.navigateTo(nextRoute);
      } else {
        // TODO: handle error
      }
    }
  }
</script>

<div>
  <div class="p-4">
    <h1 class="font-slab text-xl font-bold mb-2">Welcome to Bloggy!</h1>
    <p>
      We're excited to have you. Wondering what you're doing here? Check out
      <Link route={{ name: "support/about" }}>"What is Bloggy?"</Link>
      for a tour.
    </p>
  </div>

  <div class="p-4">
    <h2 class="font-slab text-gray-900 mb-2">Tell us your name</h2>
    <label>
      <input
        class="w-full"
        type="text"
        placeholder="Long Lizard"
        bind:value={name}
      />
    </label>
  </div>

  <div class="pb-16 bg-gray-100">
    <div class="p-4">
      <h2 class="font-slab text-gray-900 mb-2">Take your profile picture</h2>
      <p class="text-sm">
        At Bloggy we believe in <em>authenticity</em>: all users are required to
        take a profile picture with their camera when they register.
      </p>
    </div>
    <ProfilePicture bind:blob />
  </div>

  <!-- Agree to privacy policy & terms -->
  <div class="p-4">
    <label>
      <input class="mr-2" type="checkbox" bind:checked={agreed} />
      I agree to Bloggy's
      <Link route={{ name: "support/terms" }} target="_blank">
        Terms and Conditions
      </Link>
      and
      <Link route={{ name: "support/privacy" }} target="_blank">
        Privacy Policy
      </Link>.
    </label>
  </div>

  <div class="px-4 mt-8 flex justify-center items-center">
    <button
      class="c-btn w-24 h-10 bg-yellow-300 disabled:bg-gray-200 text-black disabled:text-gray-800"
      onclick={signUp}
      disabled={!isValid}
    >
      Sign up
    </button>
  </div>
</div>
