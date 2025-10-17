<script lang="ts">
  import Stage from "./stage.svelte";
  import { isMobile, isIOS } from "../utils/device";
  import Map from "./map.svelte";

  let calendarButtons!: HTMLElement;

  const instagramUrl = isMobile
    ? "instagram://user?username=i.am.caswell"
    : "https://www.instagram.com/i.am.caswell/";
  const mapsUrl = `${isIOS ? "maps" : "https"}://www.google.com/maps/dir/?api=1&destination=1869+Brownsboro+Rd%2C+Louisville%2C+KY`;

  // Style the "add-to-calendar-button" after the component has mounted
  $effect(() => {
    const calendarStyles = new CSSStyleSheet();
    calendarStyles.replaceSync(`:host(.atcb-dark) {
      --btn-padding-y: 0.375rem;
      --btn-padding-x: 1rem;
      --btn-background: var(--color-midnight-dark);
      --btn-border: transparent;
      --btn-hover-background: var(--color-midnight-medium);
      --btn-hover-border: transparent;
      --btn-text: var(--color-moon-white);
      --btn-hover-text: var(--color-moon-white);
      /* --icon-yahoo-color: #39007d; */
    }

    :host(.atcb-dark) .atcb-button {
      border-image-source: linear-gradient(to bottom, var(--color-pumpkin-red), var(--color-pumpkin-orange));
      border-image-slice: 1;
      border-width: 2px;
      border-right: 0;
      border-top: 0;
      border-bottom: 0;
    }
    `);
    calendarButtons.shadowRoot?.adoptedStyleSheets.push(calendarStyles);
  });
</script>

<svelte:head>
  <title>You're Invited!</title>
</svelte:head>

<div class="min-h-dvh bg-midnight-blue-black text-moon-white">
  <div class="mx-auto">
    <main>
      <Stage />
      <div class="py-4 font-light text-sm">
        <section id="details" class="mt-2 mb-2 px-4">
          <h2 class="c-text-gradient font-semibold font-slab text-lg">
            details
          </h2>
          <p>
            Save the date for spooky season celebration at Louisville's premier
            Halloween Party! Our 2025 party will feature new events, puzzles,
            and prizes.
          </p>
          <div class="calendar-container mt-4">
            <add-to-calendar-button
              bind:this={calendarButtons}
              name="Halloween Party"
              startDate="2025-10-25"
              startTime="19:00"
              endTime="23:59"
              timeZone="America/New_York"
              location="1869 Brownsboro Rd"
              options="'Yahoo','Google','Apple'"
              buttonStyle="flat"
              trigger="click"
              buttonsList
              hideBackground
              hideCheckmark
              size="2"
              lightMode="dark"
            ></add-to-calendar-button>
          </div>
          <div class="mt-4">
            <time datetime="2025-10-25T19:00:00">
              <strong>Saturday, October 25th</strong><br />
              Doors open at 7pm, events start at 8pm
            </time>
            <address class="not-italic mt-1">
              <a href={mapsUrl} target="_blank">
                1869 Brownsboro Rd<br />
                Louisville, KY 40206
              </a>
            </address>
          </div>
        </section>
        <section id="bring" class="mt-6 mb-2 px-4">
          <h2 class="c-text-gradient font-semibold font-slab text-lg">
            what to bring
          </h2>
          <p>
            Bevvies and light refreshments will be provided, but you can bring:
          </p>
          <ul role="list" class="list-disc pl-4">
            <li>Costume¹</li>
            <li>Friends</li>
            <li>Dogs</li>
            <li>Spooky snacks, candy and insulin²</li>
            <li>Hexes, potions and enchantments</li>
            <li>The "it" factor</li>
          </ul>
          <p class="mt-4">
            ¹<em>Guests with no costume will be publicly shamed</em><br/>
            ²<em>... Chris</em>
          </p>
        </section>
        <section id="parking" class="mt-6 mb-2 px-4">
          <h2 class="c-text-gradient font-semibold font-slab text-lg">
            where to park
          </h2>
          <p>
            Lyft and Uber are encouraged for anyone planning to drink or afraid
            of parallel parking.
            <br class="mb-[1ex]" />
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
        <section id="questions" class="mt-6 mb-6 px-4">
          <h2 class="c-text-gradient font-semibold font-slab text-lg">
            questions?
          </h2>
          <p>
            Please reach out to Luke on Instagram
            <a href={instagramUrl} target="_blank">@i.am.caswell</a>
            with any questions or special requests.
          </p>
        </section>
      </div>
    </main>
  </div>
</div>

<style>
  @keyframes wipe-on {
    0% {
      opacity: 0;
      clip-path: polygon(0 0, 0 0, 0 100%, 0 100%);
    }

    5% {
      opacity: 1;
      clip-path: polygon(0 0, 10% 0, 0 100%, 0 100%);
    }

    95% {
      opacity: 1;
      clip-path: polygon(0 0, 100% 0, 95% 100%, 0 100%);
    }

    100% {
      opacity: 1;
      clip-path: polygon(0 0, 100% 0, 100% 100%, 0 100%);
    }
  }

  #details, #bring, #parking, #questions {
    opacity: 0;
    clip-path: polygon(0 0, 0 0, 0 100%, 0 100%);
    animation: wipe-on 480ms forwards;
  }

  #details {
    animation-delay: 1s;
  }

  #bring {
    animation-delay: 1.1s;
  }

  #parking {
    animation-delay: 1.2s;
  }

  #questions {
    animation-delay: 1.3s;
  }
</style>
