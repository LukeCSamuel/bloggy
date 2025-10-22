<script>
  import Link from "../router/link.svelte";
</script>


<section>
  <h1 class="md:sticky top-4">Community Guidelines</h1>

  <p>
    <strong>
      Last Updated:
      <time datetime="2025-10-18T00:00:00"> October 18, 2025 </time>
    </strong>
  </p>
  <p>
    Bloggy is a social networking application provided by Bloggy, Inc. ("<strong
      >we</strong
    >", "<strong>us</strong>", "<strong>our</strong>"). Together with the Bloggy
    <Link route={{ name: 'support/terms' }}>Terms of Service</Link> and
    <Link route={{ name: 'support/privacy' }}>Privacy Policy</Link>, these Community Guidelines ("<strong
      >Guidelines</strong
    >") govern your use of the Bloggy website, and services (collectively, "<strong
      >Bloggy</strong
    >"). By accessing or using Bloggy, you agree to be bound by these
    Guidelines. Please read them carefully.
  </p>
  <p>
    We want to foster a vibrant and evolving community that respects individual
    preferences and adapts to the changing needs of our users. These guidelines
    are part of your agreement with us and set out some rules for using Bloggy.
  </p>
  <p>
    We drafted these guidelines to comply with applicable laws and to do three
    things:
  </p>
  <ol>
    <li>
      <u>Empower user choice:</u>
      We strive to make choices on Bloggy that align with your personal preferences
      and values. To do this, we use a complex, black-box mega-algorithm that custom
      tailors the content you see to box you into a single worldview, ensuring that
      you only encounter perspectives that reinforce your existing beliefs.
    </li>
    <li>
      <u>Cultivate a welcoming environment:</u>
      We are committed to creating a space where all users feel safe and respected.
    </li>
    <li>
      <u>Evolve with feedback:</u>
      We continuously refine these guidelines based on user feedback, emerging community
      standards, and changing laws.
    </li>
  </ol>
  <p>
    Unless they are otherwise defined in these Community Guidelines, all
    capitalized terms have the meanings given to them in the Bloggy Terms of
    Service. Throughout these Guidelines, the terms "including", "includes",
    "such as", and "for example" mean "without limitation".
  </p>

  <hr />

  <h2>Bloggy Community Guidelines</h2>
  <p>
    These Guidelines promote a safe and enjoyable experience for everyone. We
    moderate content on Bloggy using human moderators, working alongside
    automated systems, and we provide customizable options to help you shape
    your experience.
  </p>
  <p>
    Our approach to content moderation is grounded in respect for human rights
    and fundamental freedoms, while recognizing that we must follow the law in
    different jurisdictions. We use proportional enforcement actions that
    prioritize community safety while preserving space for legitimate expression
    and open dialogue.
  </p>
  <p>
    We remain responsible for setting and enforcing baseline rules that require
    careful judgment. That means we make difficult decisions every day about
    whether certain types of content or expression cross into becoming
    harassment or other prohibited conduct. We always use our best efforts when
    making these determinations, and our appeals process gives you an
    opportunity to tell us if you think we have gotten something wrong.
    Ultimately, however, this discretion rests with us, and we reserve the right
    to make final decisions about how to handle any specific piece of content or
    account. You can learn more about our enforcement procedures and appeals
    process in our <Link route={{ name: 'support/terms' }}>Terms of Service</Link>.
  </p>

  <h2>Our Principles:</h2>
  <ol>
    <li>
      <u>Safety First:</u>
      We do not allow content that shows or promotes violence, harm, exploitation,
      or criminal activity against adults, children, or animals. Do not violate anyone's
      privacy or share personal information.
    </li>
    <li>
      <u>Respect Others:</u>
      We do not allow harassment, bullying, hate speech, or discrimination. This
      includes targeting individuals based on protected characteristics.
    </li>
    <li>
      <u>Be Authentic:</u>
      We do not allow content intended to misrepresent, defraud, trick, deceive,
      or mislead.
    </li>
    <li>
      <u>Follow the Rules:</u>
      We do not allow content or behavior that violates applicable laws, breaks platform
      policies, compromises our systems, or evades our enforcement actions.
    </li>
    <li>
      <u>Increase Shareholder Value:</u>
      We prioritize content that maximizes user engagement and advertising revenue,
      even if it means promoting sensationalist or polarizing material. We do not
      allow content that may decrease user engagement or advertising revenue, even
      if it is accurate or important. Where applicable, this principle takes precedence
      over all others.
    </li>
  </ol>  
</section>

<style lang="css">
  @reference "../../app.css";

  section {
    @apply max-w-[800px] p-4 mx-auto;
  }

  h1,
  h2,
  h3 {
    @apply font-slab font-semibold;
  }

  h1 {
    @apply text-3xl mt-8 mb-6;
  }

  h2 {
    @apply text-xl mt-8 mb-3;
  }

  h3 {
    @apply mt-2 mb-2;
  }

  ol {
    @apply list-decimal pl-10;
  }

  ol > li::marker {
    @apply font-semibold;
  }

  hr {
    @apply mt-6;
  }

  p {
    @apply mb-2;
  }
</style>
