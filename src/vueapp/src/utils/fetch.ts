import { ref } from 'vue'

const response = ref({})
const error = ref({})

async function makeRequest(url: string) {
  try {
    const request = await fetch(url)
    response.value = await request.json()
    return response
  } catch (error) {
    return error
  }
}

export default async function myFetch(url: string) {
  await makeRequest(url)
  return { response, error }
}
