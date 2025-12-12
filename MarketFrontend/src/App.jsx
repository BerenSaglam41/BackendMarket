import { useEffect } from 'react'
import './App.css'
import AppRouter from './router/AppRouter'
import { useCartStore } from './store/cartStore'
import { useAuthStore } from './store/authStore'
function App() {

  const initAuth = useAuthStore((state) => state.initAuth)
  const fetchCartFromBackend = useCartStore((state) => state.fetchCartFromBackend)
  const isAuthenticated = useAuthStore((state) => state.isAuthenticated)
  
  useEffect(() => {
    initAuth()
  }, [])
  useEffect(() => {
    if (isAuthenticated) {
      fetchCartFromBackend()
    }
  }, [isAuthenticated])
  return (
   <AppRouter/>
  )
}

export default App
