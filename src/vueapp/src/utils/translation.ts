import get from 'lodash/get'
import has from 'lodash/has'
import translations from '@/assets/translations'

let activeLang: string = 'en'

type loc = { [k: string]: any }

export function getTranslation(path: string): string {
  if (has(translations, activeLang) === false) {
    activeLang = 'en'
  }

  const data = translations as loc

  return get(data[activeLang], path)
}
