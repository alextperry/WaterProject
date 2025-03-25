import { useState } from 'react'
import './App.css'
import ProjectList from './ProjectList'
import CookieConsent from "react-cookie-consent";


function App() {
  const [count, setCount] = useState(0)

  return (
    <>
      <ProjectList />
      <CookieConsent>This website uses cookies to enhance the user experience.</CookieConsent>

    </>
  )
}

export default App
