import {lazy, Suspense} from "react"
import {Box} from "@chakra-ui/react"
import {Navigate, Route, Routes} from "react-router-dom"
import "./App.css"
import SuspenseLoader from "./SuspenseLoader"
import DynamicForm from "./Pages/createNew"
import HomePage from "./Pages/homePage"
import GenerateSign from "./Pages/generateSign"
import GenerateSignPalmettoNewOrg from "./Pages/GenerateSignPalmettoNewOrg"
import GenerateSignInternal from "./Pages/GenerateSignInternal"

const Navbar = lazy(() => import("./Navbar"))
const CurtissWright = lazy(() => import("./Pages"))
const Footer = lazy(() => import("./Footer"))

function App() {
  return (
    <Suspense fallback={<SuspenseLoader />}>
      <div className="app">
        <Navbar />
        <Box style={{flex: "1 1 auto", overflow: "auto"}} bg="white">
          <Routes>
            <Route
              path="/"
              element={
                <Suspense fallback={<SuspenseLoader />}>
                  <HomePage />
                </Suspense>
              }
            />
            <Route
              path="/Generateandsign/:agreementId"
              element={
                <Suspense fallback={<SuspenseLoader />}>
                  <GenerateSignPalmettoNewOrg />
                </Suspense>
              }
            />
            <Route
              path="/GenerateandsignInternal/:agreementId"
              element={
                <Suspense fallback={<SuspenseLoader />}>
                  <GenerateSignInternal />
                </Suspense>
              }
            />
          </Routes>
        </Box>
      </div>
    </Suspense>
  )
}

export default App
