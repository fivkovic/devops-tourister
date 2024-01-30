module.exports = {
  content: ['./index.html', './src/**/*.{vue,js,ts,jsx,tsx}'],
  theme: {
    extend: {
      maxWidth: {
        '4.5xl': '60rem'
      }
    }
  },
  plugins: [require('@tailwindcss/forms'),require('@tailwindcss/line-clamp')]
}
