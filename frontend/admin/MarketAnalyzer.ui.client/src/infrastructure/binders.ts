

export function dateBinder(value: string): Date {
  return new Date(value);
}

export function numberBinder(value: string): number {
  return Number(value);
}

export function booleanBinder(value: string): boolean {
  const cast = JSON.parse(value);
  return typeof (cast) == 'boolean' ? cast : Boolean(cast);
}
