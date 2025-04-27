export interface UserModel {
    firstname: string
    lastname: string
    username: string
    email: string
    isAdmin: boolean
    isArtist: boolean
    isInactive: boolean
    inactiveTill?: string
    requirePasswordReset: boolean
    lastLogin: string
    createdOn: string
    modifiedOn?: string
    deleted: boolean
    deletedOn?: string
}

export interface LoginResponseModel {
    user: UserModel
    jwt: string
}