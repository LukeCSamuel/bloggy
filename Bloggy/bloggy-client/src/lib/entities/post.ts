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

export function getHomePostsAsync () {
  return api.getJsonAsync<PostDto[]>('api/post');
}

export function getTrendingPostsAsync () {
  return api.getJsonAsync<PostDto[]>('api/post/trending');
}

export function getPostAsync (postId: string) {
  return api.getJsonAsync<PostDto>(`api/post/${postId}`);
}

export function createPostAsync (dto: PostCreateDto) {
  return api.postJsonAsync<PostDto>('api/post', JSON.stringify(dto));
}

export function addCommentAsync (postId: string, dto: CommentCreateDto) {
  return api.postJsonAsync<Comment>(`api/post/${postId}/add-comment`, JSON.stringify(dto));
}

export function uploadImageAsync (postId: string, blob: Blob) {
  return api.postJsonAsync<Post>(`api/post/${postId}/add-image`, blob, 'image/bitmap');
}
