import {Box, Button, Flex, HStack, Text, VStack,Spinner} from "@chakra-ui/react"
// import {API_BASE_URL, BrandColors} from "../../../Constants/BrandConstants"
import {FilePlus} from "@phosphor-icons/react"
import {useEffect} from "react"
// import DotSpinner from "../../../Ui/Loading"
import {RxExit} from "react-icons/rx"
import {FaRegFileImage,FaSpinner} from "react-icons/fa"
import {RxCrossCircled} from "react-icons/rx"
import {FaCircleCheck} from "react-icons/fa6"
import {MdCancel} from "react-icons/md"
import {getToken} from "../Services/authToken"
import { base_url } from "../Services/baseUrl"
// import axios from "axios"
// import { GenerateAndSign } from "../../../Services/generateandsign"
import { useParams } from "react-router-dom"
import { composerGenerateAndSign } from "../Services/composerGenerateAndSign"
import { generateAndSignInternal } from "../Services/generateAndSignInternal"
 
const GenerateSignInternal = () => {
  //const urlParams = new URLSearchParams(window.location.search)
  //const agreementId = urlParams.get("recordId")
  const{agreementId}=useParams();
  console.log("🚀 ~ Agreement Id ~", agreementId);
  useEffect(()=>{
   
    getSigningUrl();
  },[]);
 
  const getSigningUrl=async ()=>{
   
    let accessToken=await getToken();
    console.log("Access Token",accessToken);
    let signingUrl=await generateAndSignInternal(agreementId,accessToken);
    console.log("signing Url",signingUrl);
    window.open(signingUrl);
    window.location.href=`${base_url}/clm/detail/${agreementId}`;
  }
 
  return (
    <Flex
      height="100vh"
      width="100%"
      align="center"
      justify="center"
      direction="column"
      bg="gray.50"
    >
      {/* rotating icon */}
      <Spinner
        thickness="4px"
        speed="0.8s"
        emptyColor="gray.200"
        color="blue.500"
        size="xl"
        marginBottom="20px"
      />
 
      {/* Loader text */}
      <Text fontSize="lg" fontWeight="medium" color="gray.600">
        Please wait…
      </Text>
    </Flex>
  );
}
 
export default GenerateSignInternal;