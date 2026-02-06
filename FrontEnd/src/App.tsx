import { BrowserRouter } from "react-router-dom"
import { AuthProvider } from "@/core/auth/AuthContext"
import AppRoutes from "@/routes/AppRoutes"


function App() {
  return (
    <div className='App'>
      <BrowserRouter>
        <AuthProvider>
            <AppRoutes />
        </AuthProvider>
      </BrowserRouter>
    </div>
    
  )
}

export default App
