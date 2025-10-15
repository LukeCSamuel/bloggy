<script lang="ts">
  import Stage from "./stage.svelte";
  import { isMobile, isIOS } from "../utils/device";
  import Map from "./map.svelte";

  const instagramUrl = isMobile
    ? "instagram://user?username=i.am.caswell"
    : "https://www.instagram.com/i.am.caswell/";
  const mapsUrl = `${isIOS ? "maps" : "https"}://www.google.com/maps/dir/?api=1&destination=1869+Brownsboro+Rd%2C+Louisville%2C+KY`;
</script>

<svelte:head>
  <title>You're Invited!</title>
</svelte:head>

<div class="min-h-dvh bg-midnight-blue-black text-moon-white">
  <div class="mx-auto">
    <main>
      <Stage />
      <div class="p-4 font-light text-sm">
        <section id="details" class="mt-2 mb-2">
          <h2 class="c-text-gradient font-semibold font-slab text-lg">details</h2>
          <p>
            <em>You're invited to celebrate spooky season!</em>
          </p>
          <div class="mt-4">
            <time datetime="2025-10-25T19:00:00">
              Starting 7pm | Saturday, October 25th
            </time>
            <address class="not-italic">
              <a href={mapsUrl} target="_blank">
                1869 Brownsboro Rd<br />
                Louisville, KY 40206
              </a>
            </address>
          </div>
        </section>
        <section id="bring" class="mt-6 mb-2">
          <h2 class="c-text-gradient font-semibold font-slab text-lg">what to bring</h2>
          <ul role="list" class="list-disc pl-4">
            <li>Friends</li>
            <li>Spooky snacks, candy, insulin</li>
            <li>Costume (mandatory)</li>
            <li>Witchy vibes</li>
            <li>Dogs</li>
          </ul>
          <p class="mt-4">
            <em>Drinks will be provided.</em>
          </p>
        </section>
        <section id="parking" class="mt-6 mb-2">
          <h2 class="c-text-gradient font-semibold font-slab text-lg">where to park</h2>
          <p>
            Lyft and Uber are encouraged for anyone drinking alcohol or afraid
            of parallel parking.
            <br />
            Limited parking is available on the North side of Brownsboro Rd. Additional
            parking can be found on State St., Drescher Bridge Ave. and Pope St.
          </p>
          <div class="my-2">
            <Map />
          </div>
          <p>
            <em>
              Driveway space is reserved for cars wearing Halloween costumes
              <strong>only!</strong>
              Do not park in the driveway if your car doesn't have the haunting spirit.
            </em>
          </p>
        </section>
        <section id="questions" class="mt-6 mb-2">
          <h2 class="c-text-gradient font-semibold font-slab text-lg">questions?</h2>
          <p>
            Please reach out to Luke on Instagram
            <a href={instagramUrl} target="_blank">@i.am.caswell</a>
            with any questions or special requests.
          </p>
        </section>
        <div>
          <!-- TODO: Calendar -->
          <!--
          <add-to-calendar-button
            name="Halloween Party"
            startDate="2025-10-25"
            startTime="19:00"
            endTime="23:59"
            timeZone="America/New_York"
            location="1869 Brownsboro Rd"
            options="'Google','Apple'"
            buttonStyle="round"
            trigger="click"
            buttonsList
            hideBackground
            hideCheckmark
            size="3"
            lightMode="dark"
          ></add-to-calendar-button>
          -->
        </div>
      </div>
    </main>
  </div>
</div>
