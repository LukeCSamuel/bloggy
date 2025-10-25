import { alertContainer } from '../utils/alerts.svelte';
import { api } from '../utils/api';

export interface SuccessfulCompletion {
  name: string;
  time: string;
  pointsAwarded: number;
}

export interface FailedCompletion {
  name?: string;
  isExpired?: boolean;
  pointsAwarded: 0;
}

export type CompletionResponse = SuccessfulCompletion | FailedCompletion;

export type CompletionRequest =
  | {
    key: string;
    eventId: string;
  }
  | {
    key: string;
    challengeId: string;
  };

export function isCompletion (obj: object | undefined): obj is CompletionResponse {
  return obj !== undefined && 'pointsAwarded' in obj;
}

export function isFailedCompletion (obj: object | undefined): obj is FailedCompletion {
  return isCompletion(obj) && obj.pointsAwarded === 0 && !('time' in obj);
}

export async function tryCompletionAsync (req: CompletionRequest) {
  const result = await api.postJsonAsync<CompletionResponse>('api/completion', JSON.stringify(req));
  if (isCompletion(result) && result.pointsAwarded) {
    alertContainer.addAlert({
      completion: result,
    }, 5000);
  }
}
