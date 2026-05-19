import {ChakraProvider, extendTheme} from "@chakra-ui/react"
import {createRoot} from "react-dom/client"
import {BrowserRouter} from "react-router-dom"
import "./index.css"
import App from "./App.jsx"

const theme = extendTheme({
  fonts: {body: "Poppins", heading: "Poppins, serif", mono: "Poppins, monospace"},
  components: {
    Checkbox: {
      baseStyle: {
        control: {
          _checked: {
            bg: "black.500",
            borderColor: "#0170ef",
            color: "#0170ef", 
            _hover: {
              bg: "transparent",
              borderColor: "currentColor"
            }
          }
        }
      }
    }
  }
})

createRoot(document.getElementById("root")).render(
  <ChakraProvider theme={theme} resetCSS>
    <BrowserRouter>
      <App />
    </BrowserRouter>
  </ChakraProvider>
)
