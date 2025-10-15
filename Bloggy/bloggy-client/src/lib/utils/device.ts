export const isMobile = globalThis.matchMedia('(pointer: coarse)').matches;
export const isIOS = /iPad|iPhone|iPod/.test(navigator.userAgent);

export const device = {
  isMobile,
  isIOS,
};
