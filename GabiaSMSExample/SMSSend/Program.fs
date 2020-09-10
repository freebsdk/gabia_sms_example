open GabiaSmsAgent




let GabiaID =
    "(gabia login id)"
    
let GabiaAPIKey =
    "(issued api key)"
    
let ReceivePhoneNumber =
    "(phone number)"

let CallbackPhoneNumber =
    "(phone number)"


    
[<EntryPoint>]
let main argv =
    let oauth_result = GabiaSMS.Login <| GabiaID <| GabiaAPIKey
    let sms_result = GabiaSMS.SendMessage
                     <| GabiaID
                     <| oauth_result.access_token
                     <| ReceivePhoneNumber
                     <| CallbackPhoneNumber
                     <| "Hello. SMS World"
                     <| GabiaSMS.DefaultRefKey()
                     <| "n"
    0