import { api } from '../utils/api';
import type { EntityBase } from './entity-base';
import type { User } from './user';

export interface Post extends EntityBase {
  title: string;
  images: string[];
  text: string;
  created: string;
  isTrending: boolean;
}

export interface Comment extends EntityBase {
  postId: string;
  text: string;
  created: string;
}

export interface PostDto {
  post: Post;
  comments: Comment[];
  authors: User[];
}

export interface PostCreateDto {
  title: string;
  text: string;
}

export interface CommentCreateDto {
  text: string;
}

export async function getHomePostsAsync () {
  return await api.getJsonAsync<PostDto[]>('api/post');
}

export async function getTrendingPostsAsync () {
  return await api.getJsonAsync<PostDto[]>('api/post/trending');
}

export async function createPostAsync (dto: PostCreateDto) {
  return await api.postJsonAsync<PostDto>('api/post', JSON.stringify(dto));
}

export async function addCommentAsync (postId: string, dto: CommentCreateDto) {
  return await api.postJsonAsync<Comment>(`api/post/${postId}/add-comment`, JSON.stringify(dto));
}

export async function uploadImageAsync (postId: string, blob: Blob) {
  return await api.postJsonAsync<Post>(`api/post/${postId}/add-image`, blob, 'image/bitmap');
}
