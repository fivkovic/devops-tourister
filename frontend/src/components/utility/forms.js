const formData = event => {
  return Object.fromEntries(new FormData(event.target).entries())
}

const checkForEmptyFields = data => {
  return Object.values(data).some(x => x == '' || x == null)
}

const debounce = (fn, delay) => {
  var timeoutID = null
  return function () {
    clearTimeout(timeoutID)
    var args = arguments
    var that = this
    timeoutID = setTimeout(() => {
      fn.apply(that, args)
    }, delay)
  }
}

export { formData, checkForEmptyFields, debounce }
