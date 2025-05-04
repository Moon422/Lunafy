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
    id: number
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
    isEditable: boolean
}