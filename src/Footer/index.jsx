import {Center, Image} from "@chakra-ui/react"
import logo from "../assets/logo2.jpg"

const Footer = () => {
  return (
    <Center fontSize={"sm"} padding={2} gap={1}>
      <Image height="50px" objectFit={"contain"} src={logo} alt="logo" />
    </Center>
  )
}

export default Footer
