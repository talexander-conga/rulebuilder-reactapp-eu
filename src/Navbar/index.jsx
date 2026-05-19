/* eslint-disable no-unused-vars */
import {Box, Image, Text} from "@chakra-ui/react"

const Navbar = () => {
  return (
    <Box display={"flex"} justifyContent={"start"} flexWrap={"wrap"} id="navbar" flex={"0 0 auto"} className="navbar" alignItems="center" gap="2" paddingInline={"1rem"} paddingBlock={"5px"} paddingLeft={"2rem !important"} bg="transparent">
      <Image src={"https://conga.com/themes/custom/themekit/images/branding/favicon-conga.ico"} margin={"0"} alt="logo" cursor={"pointer"} objectFit={"contain"} height={"2rem"} />
      <Text color={"#072034"} fontWeight={"400"} fontSize={"1.5rem"} lineHeight={"36px"} letterSpacing={0.25} cursor={"default"} fontFamily={"Open Sans"}>
        Contract Apps
      </Text>
    </Box>
  )
}

export default Navbar
