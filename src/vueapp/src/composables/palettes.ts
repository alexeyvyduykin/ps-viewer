/* eslint-disable @typescript-eslint/no-unused-vars */
import chroma from 'chroma-js'

export enum ColorPalettes {
  Default,
  RetroMetro,
  DutchField,
  RiverNights,
  SpringPastels
}

export function useColorPalette(palette?: ColorPalettes): string[] {
  switch (palette) {
    case ColorPalettes.Default:
      return defaultPalette
    case ColorPalettes.RetroMetro:
      return retroMetroPalette
    case ColorPalettes.DutchField:
      return dutchFieldPalette
    case ColorPalettes.RiverNights:
      return riverNightsPalette
    case ColorPalettes.SpringPastels:
      return springPastelsPalette
    default:
      break
  }

  return []
}

function componentToHex(c: number) {
  const hex = c.toString(16)
  return hex.length == 1 ? '0' + hex : hex
}

export function useRgbToHex(r: number, g: number, b: number): string {
  return '#' + componentToHex(r) + componentToHex(g) + componentToHex(b)
}

const defaultPalette = [
  '#0B84A5', // Color.FromArgb(11, 132, 165), // #0B84A5
  '#F6C85F', // Color.FromArgb(246, 200, 95), // #F6C85F
  '#6F4E7C', // Color.FromArgb(111, 78, 124), // #6F4E7C
  '#9DD866', // Color.FromArgb(157, 216, 102), // #9DD866
  '#CA472F', // Color.FromArgb(202, 71, 47), // #CA472F
  '#FFA056', // Color.FromArgb(255, 160, 86), // #FFA056
  '#8DDDD0' // Color.FromArgb(141, 221, 208), // #8DDDD0
]

const retroMetroPalette = [
  '#ea5545', // Color.FromArgb(234, 85, 69), // #ea5545
  '#f46a9b', // Color.FromArgb(244, 106, 155), // #f46a9b
  '#ef9b20', // Color.FromArgb(239, 155, 32), // #ef9b20
  '#edbf33', // Color.FromArgb(237, 191, 51), // #edbf33
  '#ede15b', // Color.FromArgb(237, 225, 91), // #ede15b
  '#bdcf32', // Color.FromArgb(189, 207, 50), // #bdcf32
  '#87bc45', // Color.FromArgb(135, 188, 69), // #87bc45
  '#27aeef', // Color.FromArgb(39, 174, 239), // #27aeef
  '#b33dc6' // Color.FromArgb(179, 61, 198), // #b33dc6
]

const dutchFieldPalette = [
  '#e60049', // Color.FromArgb(230, 0, 73), // #e60049
  '#0bb4ff', // Color.FromArgb(11, 180, 255), // #0bb4ff
  '#50e991', // Color.FromArgb(80, 233, 145), // #50e991
  '#e6d800', // Color.FromArgb(230, 216, 0), // #e6d800
  '#9b19f5', // Color.FromArgb(155, 25, 245), // #9b19f5
  '#ffa300', // Color.FromArgb(255, 163, 0), // #ffa300
  '#dc0ab4', // Color.FromArgb(220, 10, 180), // #dc0ab4
  '#b3d4ff', // Color.FromArgb(179, 212, 255), // #b3d4ff
  '#00bfa0' // Color.FromArgb(0, 191, 160), // #00bfa0
]

const riverNightsPalette = [
  '#b30000', // Color.FromArgb(179, 0, 0), // #b30000
  '#7c1158', // Color.FromArgb(124, 17, 88), // #7c1158
  '#4421af', // Color.FromArgb(68, 33, 175), // #4421af
  '#1a53ff', // Color.FromArgb(26, 83, 255), // #1a53ff
  '#0d88e6', // Color.FromArgb(13, 136, 230), // #0d88e6
  '#00b7c7', // Color.FromArgb(0, 183, 199), // #00b7c7
  '#5ad45a', // Color.FromArgb(90, 212, 90), // #5ad45a
  '#8be04e', // Color.FromArgb(139, 224, 78), // #8be04e
  '#ebdc78' // Color.FromArgb(235, 220, 120), // #ebdc78
]

const springPastelsPalette = [
  '#fd7f6f', // Color.FromArgb(253, 127, 111), // #fd7f6f
  '#7eb0d5', // Color.FromArgb(126, 176, 213), // #7eb0d5
  '#b2e061', // Color.FromArgb(178, 224, 97), // #b2e061
  '#bd7ebe', // Color.FromArgb(189, 126, 190), // #bd7ebe
  '#ffb55a', // Color.FromArgb(255, 181, 90), // #ffb55a
  '#ffee65', // Color.FromArgb(255, 238, 101), // #ffee65
  '#beb9db', // Color.FromArgb(190, 185, 219), // #beb9db
  '#fdcce5', // Color.FromArgb(253, 204, 229), // #fdcce5
  '#8bd3c7' // Color.FromArgb(139, 211, 199), // #8bd3c7
]

export type ColorHuePalette = { [key: number]: string[] }

const minCount = 3
const maxCount = 9

export function useFromColorHuePalette(index: number, count: number): string {
  const num = Math.max(minCount, Math.min(maxCount, count))
  const i = Math.max(0, Math.min(num - 1, index))

  return chroma.scale('Purples').colors(num)[num - i - 1]

  //return purplesHuePalette[num][num - 1 - i];
}

const orangesHuePalette: ColorHuePalette = {
  1: [useRgbToHex(230, 85, 13)],
  2: [useRgbToHex(253, 174, 107), useRgbToHex(230, 85, 13)],
  3: [useRgbToHex(254, 230, 206), useRgbToHex(253, 174, 107), useRgbToHex(230, 85, 13)],
  4: [
    useRgbToHex(254, 237, 222),
    useRgbToHex(253, 190, 133),
    useRgbToHex(253, 141, 60),
    useRgbToHex(217, 71, 1)
  ],
  5: [
    useRgbToHex(254, 237, 222),
    useRgbToHex(253, 190, 133),
    useRgbToHex(253, 141, 60),
    useRgbToHex(230, 85, 13),
    useRgbToHex(166, 54, 3)
  ],
  6: [
    useRgbToHex(254, 237, 222),
    useRgbToHex(253, 208, 162),
    useRgbToHex(253, 174, 107),
    useRgbToHex(253, 141, 60),
    useRgbToHex(230, 85, 13),
    useRgbToHex(166, 54, 3)
  ],
  7: [
    useRgbToHex(254, 237, 222),
    useRgbToHex(253, 208, 162),
    useRgbToHex(253, 174, 107),
    useRgbToHex(253, 141, 60),
    useRgbToHex(241, 105, 19),
    useRgbToHex(217, 72, 1),
    useRgbToHex(140, 45, 4)
  ],
  8: [
    useRgbToHex(255, 245, 235),
    useRgbToHex(254, 230, 206),
    useRgbToHex(253, 208, 162),
    useRgbToHex(253, 174, 107),
    useRgbToHex(253, 141, 60),
    useRgbToHex(241, 105, 19),
    useRgbToHex(217, 72, 1),
    useRgbToHex(140, 45, 4)
  ],
  9: [
    useRgbToHex(255, 245, 235),
    useRgbToHex(254, 230, 206),
    useRgbToHex(253, 208, 162),
    useRgbToHex(253, 174, 107),
    useRgbToHex(253, 141, 60),
    useRgbToHex(241, 105, 19),
    useRgbToHex(217, 72, 1),
    useRgbToHex(166, 54, 3),
    useRgbToHex(127, 39, 4)
  ]
}

const redsHuePalette: ColorHuePalette = {
  1: [useRgbToHex(222, 45, 38)],
  2: [useRgbToHex(252, 146, 114), useRgbToHex(222, 45, 38)],
  3: [useRgbToHex(254, 224, 210), useRgbToHex(252, 146, 114), useRgbToHex(222, 45, 38)],
  4: [
    useRgbToHex(254, 229, 217),
    useRgbToHex(252, 174, 145),
    useRgbToHex(251, 106, 74),
    useRgbToHex(203, 24, 29)
  ],
  5: [
    useRgbToHex(254, 229, 217),
    useRgbToHex(252, 174, 145),
    useRgbToHex(251, 106, 74),
    useRgbToHex(222, 45, 38),
    useRgbToHex(165, 15, 21)
  ],
  6: [
    useRgbToHex(254, 229, 217),
    useRgbToHex(252, 187, 161),
    useRgbToHex(252, 146, 114),
    useRgbToHex(251, 106, 74),
    useRgbToHex(222, 45, 38),
    useRgbToHex(165, 15, 21)
  ],
  7: [
    useRgbToHex(254, 229, 217),
    useRgbToHex(252, 187, 161),
    useRgbToHex(252, 146, 114),
    useRgbToHex(251, 106, 74),
    useRgbToHex(239, 59, 44),
    useRgbToHex(203, 24, 29),
    useRgbToHex(153, 0, 13)
  ],
  8: [
    useRgbToHex(255, 245, 240),
    useRgbToHex(254, 224, 210),
    useRgbToHex(252, 187, 161),
    useRgbToHex(252, 146, 114),
    useRgbToHex(251, 106, 74),
    useRgbToHex(239, 59, 44),
    useRgbToHex(203, 24, 29),
    useRgbToHex(153, 0, 13)
  ],
  9: [
    useRgbToHex(255, 245, 240),
    useRgbToHex(254, 224, 210),
    useRgbToHex(252, 187, 161),
    useRgbToHex(252, 146, 114),
    useRgbToHex(251, 106, 74),
    useRgbToHex(239, 59, 44),
    useRgbToHex(203, 24, 29),
    useRgbToHex(165, 15, 21),
    useRgbToHex(103, 0, 13)
  ]
}

const greensHuePalette: ColorHuePalette = {
  1: [useRgbToHex(49, 163, 84)],
  2: [useRgbToHex(161, 217, 155), useRgbToHex(49, 163, 84)],
  3: [useRgbToHex(229, 245, 224), useRgbToHex(161, 217, 155), useRgbToHex(49, 163, 84)],
  4: [
    useRgbToHex(237, 248, 233),
    useRgbToHex(186, 228, 179),
    useRgbToHex(116, 196, 118),
    useRgbToHex(35, 139, 69)
  ],
  5: [
    useRgbToHex(237, 248, 233),
    useRgbToHex(186, 228, 179),
    useRgbToHex(116, 196, 118),
    useRgbToHex(49, 163, 84),
    useRgbToHex(0, 109, 44)
  ],
  6: [
    useRgbToHex(237, 248, 233),
    useRgbToHex(199, 233, 192),
    useRgbToHex(161, 217, 155),
    useRgbToHex(116, 196, 118),
    useRgbToHex(49, 163, 84),
    useRgbToHex(0, 109, 44)
  ],
  7: [
    useRgbToHex(237, 248, 233),
    useRgbToHex(199, 233, 192),
    useRgbToHex(161, 217, 155),
    useRgbToHex(116, 196, 118),
    useRgbToHex(65, 171, 93),
    useRgbToHex(35, 139, 69),
    useRgbToHex(0, 90, 50)
  ],
  8: [
    useRgbToHex(247, 252, 245),
    useRgbToHex(229, 245, 224),
    useRgbToHex(199, 233, 192),
    useRgbToHex(161, 217, 155),
    useRgbToHex(116, 196, 118),
    useRgbToHex(65, 171, 93),
    useRgbToHex(35, 139, 69),
    useRgbToHex(0, 90, 50)
  ],
  9: [
    useRgbToHex(247, 252, 245),
    useRgbToHex(229, 245, 224),
    useRgbToHex(199, 233, 192),
    useRgbToHex(161, 217, 155),
    useRgbToHex(116, 196, 118),
    useRgbToHex(65, 171, 93),
    useRgbToHex(35, 139, 69),
    useRgbToHex(0, 109, 44),
    useRgbToHex(0, 68, 27)
  ]
}

const purplesHuePalette: ColorHuePalette = {
  1: [useRgbToHex(117, 107, 177)],
  2: [useRgbToHex(188, 189, 220), useRgbToHex(117, 107, 177)],
  3: [useRgbToHex(239, 237, 245), useRgbToHex(188, 189, 220), useRgbToHex(117, 107, 177)],
  4: [
    useRgbToHex(242, 240, 247),
    useRgbToHex(203, 201, 226),
    useRgbToHex(158, 154, 200),
    useRgbToHex(106, 81, 163)
  ],
  5: [
    useRgbToHex(242, 240, 247),
    useRgbToHex(203, 201, 226),
    useRgbToHex(158, 154, 200),
    useRgbToHex(117, 107, 177),
    useRgbToHex(84, 39, 143)
  ],
  6: [
    useRgbToHex(242, 240, 247),
    useRgbToHex(218, 218, 235),
    useRgbToHex(188, 189, 220),
    useRgbToHex(158, 154, 200),
    useRgbToHex(117, 107, 177),
    useRgbToHex(84, 39, 143)
  ],
  7: [
    useRgbToHex(242, 240, 247),
    useRgbToHex(218, 218, 235),
    useRgbToHex(188, 189, 220),
    useRgbToHex(158, 154, 200),
    useRgbToHex(128, 125, 186),
    useRgbToHex(106, 81, 163),
    useRgbToHex(74, 20, 134)
  ],
  8: [
    useRgbToHex(252, 251, 253),
    useRgbToHex(239, 237, 245),
    useRgbToHex(218, 218, 235),
    useRgbToHex(188, 189, 220),
    useRgbToHex(158, 154, 200),
    useRgbToHex(128, 125, 186),
    useRgbToHex(106, 81, 163),
    useRgbToHex(74, 20, 134)
  ],
  9: [
    useRgbToHex(252, 251, 253),
    useRgbToHex(239, 237, 245),
    useRgbToHex(218, 218, 235),
    useRgbToHex(188, 189, 220),
    useRgbToHex(158, 154, 200),
    useRgbToHex(128, 125, 186),
    useRgbToHex(106, 81, 163),
    useRgbToHex(84, 39, 143),
    useRgbToHex(63, 0, 125)
  ]
}

const greysHuePalette: ColorHuePalette = {
  1: [useRgbToHex(99, 99, 99)],
  2: [useRgbToHex(189, 189, 189), useRgbToHex(99, 99, 99)],
  3: [useRgbToHex(240, 240, 240), useRgbToHex(189, 189, 189), useRgbToHex(99, 99, 99)],
  4: [
    useRgbToHex(247, 247, 247),
    useRgbToHex(204, 204, 204),
    useRgbToHex(150, 150, 150),
    useRgbToHex(82, 82, 82)
  ],
  5: [
    useRgbToHex(247, 247, 247),
    useRgbToHex(204, 204, 204),
    useRgbToHex(150, 150, 150),
    useRgbToHex(99, 99, 99),
    useRgbToHex(37, 37, 37)
  ],
  6: [
    useRgbToHex(247, 247, 247),
    useRgbToHex(217, 217, 217),
    useRgbToHex(189, 189, 189),
    useRgbToHex(150, 150, 150),
    useRgbToHex(99, 99, 99),
    useRgbToHex(37, 37, 37)
  ],
  7: [
    useRgbToHex(247, 247, 247),
    useRgbToHex(217, 217, 217),
    useRgbToHex(189, 189, 189),
    useRgbToHex(150, 150, 150),
    useRgbToHex(115, 115, 115),
    useRgbToHex(82, 82, 82),
    useRgbToHex(37, 37, 37)
  ],
  8: [
    useRgbToHex(255, 255, 255),
    useRgbToHex(240, 240, 240),
    useRgbToHex(217, 217, 217),
    useRgbToHex(189, 189, 189),
    useRgbToHex(150, 150, 150),
    useRgbToHex(115, 115, 115),
    useRgbToHex(82, 82, 82),
    useRgbToHex(37, 37, 37)
  ],
  9: [
    useRgbToHex(255, 255, 255),
    useRgbToHex(240, 240, 240),
    useRgbToHex(217, 217, 217),
    useRgbToHex(189, 189, 189),
    useRgbToHex(150, 150, 150),
    useRgbToHex(115, 115, 115),
    useRgbToHex(82, 82, 82),
    useRgbToHex(37, 37, 37),
    useRgbToHex(0, 0, 0)
  ]
}

const bluesHuePalette: ColorHuePalette = {
  1: [useRgbToHex(49, 130, 189)],
  2: [useRgbToHex(158, 202, 225), useRgbToHex(49, 130, 189)],
  3: [useRgbToHex(222, 235, 247), useRgbToHex(158, 202, 225), useRgbToHex(49, 130, 189)],
  4: [
    useRgbToHex(239, 243, 255),
    useRgbToHex(189, 215, 231),
    useRgbToHex(107, 174, 214),
    useRgbToHex(33, 113, 181)
  ],
  5: [
    useRgbToHex(239, 243, 255),
    useRgbToHex(189, 215, 231),
    useRgbToHex(107, 174, 214),
    useRgbToHex(49, 130, 189),
    useRgbToHex(8, 81, 156)
  ],
  6: [
    useRgbToHex(239, 243, 255),
    useRgbToHex(198, 219, 239),
    useRgbToHex(158, 202, 225),
    useRgbToHex(107, 174, 214),
    useRgbToHex(49, 130, 189),
    useRgbToHex(8, 81, 156)
  ],
  7: [
    useRgbToHex(239, 243, 255),
    useRgbToHex(198, 219, 239),
    useRgbToHex(158, 202, 225),
    useRgbToHex(107, 174, 214),
    useRgbToHex(66, 146, 198),
    useRgbToHex(33, 113, 181),
    useRgbToHex(8, 69, 148)
  ],
  8: [
    useRgbToHex(247, 251, 255),
    useRgbToHex(222, 235, 247),
    useRgbToHex(198, 219, 239),
    useRgbToHex(158, 202, 225),
    useRgbToHex(107, 174, 214),
    useRgbToHex(66, 146, 198),
    useRgbToHex(33, 113, 181),
    useRgbToHex(8, 69, 148)
  ],
  9: [
    useRgbToHex(247, 251, 255),
    useRgbToHex(222, 235, 247),
    useRgbToHex(198, 219, 239),
    useRgbToHex(158, 202, 225),
    useRgbToHex(107, 174, 214),
    useRgbToHex(66, 146, 198),
    useRgbToHex(33, 113, 181),
    useRgbToHex(8, 81, 156),
    useRgbToHex(8, 48, 107)
  ]
}
