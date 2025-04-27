export interface TotalUsersStatApiResponse {
    presentDataTill?: string
    presentUserCount: number
    previousDataTill?: string
    previousUserCount: number
    infiniteIncrement: boolean
    didUserIncremented?: boolean
    changePercentage: number
}