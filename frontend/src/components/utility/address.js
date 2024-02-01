const formatAddress = address => {
  return `${address.street} ${address.apartment} ${address.city}, ${address.country}`
}

export { formatAddress }
