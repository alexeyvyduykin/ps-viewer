import { toRefs, reactive, computed } from 'vue'

const palette = [
  { key: 1, values: ['#756BB1'] },
  { key: 2, values: ['#BCBDDC', '#756BB1'] },
  { key: 3, values: ['#EFEDF5', '#BCBDDC', '#756BB1'] },
  { key: 4, values: ['#F2F0F7', '#CBC9E2', '#9E9AC8', '#6A51A3'] },
  { key: 5, values: ['#F2F0F7', '#CBC9E2', '#9E9AC8', '#756BB1', '#54278F'] },
  { key: 6, values: ['#F2F0F7', '#DADAEB', '#BCBDDC', '#9E9AC8', '#756BB1', '#54278F'] },
  {
    key: 7,
    values: ['#F2F0F7', '#DADAEB', '#BCBDDC', '#9E9AC8', '#807DBA', '#6A51A3', '#4A1486']
  },
  {
    key: 8,
    values: ['#FCFBFD', '#EFEDF5', '#DADAEB', '#BCBDDC', '#9E9AC8', '#807DBA', '#6A51A3', '#4A1486']
  },
  {
    key: 9,
    values: [
      '#FCFBFD',
      '#EFEDF5',
      '#DADAEB',
      '#BCBDDC',
      '#9E9AC8',
      '#807DBA',
      '#6A51A3',
      '#54278F',
      '#3F007D'
    ]
  }
]

const availableAreaCounts = [1, 2, 3, 4, 5]
const availableCountModes = ['None', 'Equal', 'Geometric']

export interface ColorRect {
  index: number
  color: string
  angle: number
}

// export function createColorRect(angle: number, index: number, count: number): ColorRect {
//   const color = getColor(index, count);
//   return {
//     index: index,
//     color: color,
//     angle: angle,
//   };
// }

// export function getColor(index: number, number: number): string {
//   const minNumber = 1;
//   const maxNumber = 9;
//   const num = Math.max(minNumber, Math.min(maxNumber, number));
//   const i = Math.max(0, Math.min(num - 1, index));

//   const values = palette[num].values;
//   return values[num - 1 - i];
//   //return palette[num][num - 1 - i] as string;
// }

export function createAngles(
  _innerAngle: number,
  _outerAngle: number,
  _areaCount: number,
  countMode: string
): number[] {
  if (countMode === 'Equal') {
    return createAreaItemsEqualMode(_innerAngle, _outerAngle, _areaCount)
  } else if (countMode === 'Geometric') {
    return createAreaItemsGeometricMode(_innerAngle, _outerAngle, _areaCount)
  }

  return []
}

function createAreaItemsEqualMode(inner: number, outer: number, areaCount: number): number[] {
  const areaStep = (outer - inner) / areaCount

  const list = Array<number>()

  for (let i = 0; i < areaCount; i++) {
    const angle = inner + areaStep * (i + 1)
    list.push(angle)
  }

  return list
}

function createAreaItemsGeometricMode(inner: number, outer: number, areaCount: number): number[] {
  const list = Array<number>()

  for (let i = 0; i < areaCount; i++) {
    const step = (outer - inner) * getGeometricStep(i + 1, areaCount)

    const angle = inner + step

    list.push(angle)
  }

  return list
}

function getGeometricStep(n: number, count: number): number {
  if (n == count) {
    return 1.0
  }

  let value = 0.0

  for (let i = 0; i < n; i++) {
    value += 1 / Math.pow(2, i + 1)
  }

  return value
}
