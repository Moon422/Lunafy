export interface TotalUsersStatApiResponse {
    presentDataTill?: string
    presentUserCount: number
    previousDataTill?: string
    previousUserCount: number
    infiniteIncrement: boolean
    didUserIncremented?: boolean
    changePercentage: number
}

export interface UserModel {
    firstname: string
    lastname: string
    username: string
    email: string
    isAdmin: boolean
    isArtist: boolean
}

export interface UserCreateModel extends UserModel { }

export interface UserReadModel extends UserModel {
    id: number
    isInactive: boolean
    inactiveTill?: string
    requirePasswordReset: boolean
    lastLogin: string
    createdOn: string
    modifiedOn?: string
    deleted: boolean
    deletedOn?: string
    isEditable: boolean
}