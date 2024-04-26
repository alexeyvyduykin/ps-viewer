declare global {
  interface Math {
    toRadians(degrees: number): number
    toDegrees(radians: number): number
  }
}

export {}
