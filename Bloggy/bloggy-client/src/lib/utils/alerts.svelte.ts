import type { Snippet } from 'svelte';
import type { CompletionResponse } from '../entities/completion';

export interface TextAlert {
  kind: 'error' | 'success';
  text: string | Snippet;
}

export interface CompletionAlert {
  completion: CompletionResponse;
}

export type Alert =
  | TextAlert
  | CompletionAlert;

export function isTextAlert (a: Alert): a is TextAlert {
  return 'kind' in a;
}

export type WithId<T> = T & { id: string };

const _alerts: WithId<Alert>[] = $state([]);

class AlertState {
  readonly alerts = $derived(_alerts);

  addAlert (alert: Alert, duration?: number) {
    const actual = {
      ...alert,
      id: crypto.randomUUID(),
    };

    _alerts.push(actual);
    setTimeout(() => {
      this.clearAlert(actual);
    }, duration ?? 3000);
  }

  clearAlert (alert: WithId<Alert>) {
    const index = _alerts.findIndex(a => a.id === alert.id);
    _alerts.splice(index, 1);
  }
}

export const alertContainer = new AlertState();
