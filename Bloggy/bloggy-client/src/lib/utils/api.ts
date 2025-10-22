export const apiBase = new URL(document.baseURI).origin;

export const TOKEN_KEY = 'User.Jwt';

class BloggyApi {
  async getJsonAsync<T> (path: string) {
    const result = await fetch(
      new URL(path, apiBase),
      {
        method: 'GET',
        headers: this.getAuthHeader(),
      },
    );

    if (result.ok) {
      return result.json() as T;
    }
  }

  async postJsonAsync<T> (path: string, body: BodyInit, contentType = 'application/json') {
    const result = await fetch(
      new URL(path, apiBase),
      {
        method: 'POST',
        headers: {
          ...this.getAuthHeader(),
          'Content-Type': contentType,
        },
        body,
      },
    );

    if (result.ok) {
      return result.json() as T;
    }
  }

  getAuthHeader (): Record<string, string> {
    const token = localStorage.getItem(TOKEN_KEY);
    if (token) {
      return {
        Authorization: `Bearer ${token}`,
      };
    } else {
      return {};
    }
  }
}

export const api = new BloggyApi();
