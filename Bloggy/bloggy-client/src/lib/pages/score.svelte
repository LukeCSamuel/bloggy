<script lang="ts">
  import Loader from "../components/loader.svelte";
  import ScoreBar from "../components/score-bar.svelte";
  import { getScoresAsync, type UserCompletion } from "../entities/completion";

  let scoresPromise = $derived(getScoresAsync());

  function maxScore(scores: UserCompletion[]) {
    // TODO: it would be ideal to memoize this computation since it's used a lot
    const totals = scores.map((score) =>
      score.completions.reduce(
        (i, completion) => i + completion.pointsAwarded,
        0,
      ),
    );
    return totals.reduce((max, i) => Math.max(max, i), 0);
  }

  function order(scores: UserCompletion[]) {
    scores.sort(
      (a, b) =>
        b.completions.reduce(
          (i, completion) => i + completion.pointsAwarded,
          0,
        ) -
        a.completions.reduce(
          (i, completion) => i + completion.pointsAwarded,
          0,
        ),
    );
    return scores;
  }
</script>

<div class="p-4">
  <Loader promise={scoresPromise}>
    {#snippet success(scores)}
      {@const max = maxScore(scores)}
      {@const ordered = order(scores) }
      <div class="c-score-grid gap-4">
        {#each ordered as score}
          <ScoreBar user={score.user} completions={score.completions} {max} />
        {/each}
      </div>
    {/snippet}
  </Loader>
</div>

<style lang="css">
  @reference "../../app.css";

  .c-score-grid {
    grid-template-columns: max-content 1fr;
    @apply grid;
  }
</style>
