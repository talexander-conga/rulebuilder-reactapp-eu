import {Flex, Spinner} from "@chakra-ui/react"

const SuspenseLoader = () => {
  return (
    <Flex minHeight={"calc(100vh - 106px)"} height={"100%"} width={"100%"} alignItems="center" justifyContent={"center"}>
      <Spinner thickness="4px" speed="0.65s" emptyColor="gray.200" color={"#0170ef"} size="lg" />
    </Flex>
  )
}

export default SuspenseLoader
